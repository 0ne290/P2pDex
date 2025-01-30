using System.Timers;
using Core.Domain.Constants;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace Core.Application;

public class OrderTransferTransactionTracker : IDisposable
{
    public OrderTransferTransactionTracker(IBlockchain blockchain, IUnitOfWork unitOfWork,
        ExchangerConfiguration exchangerConfiguration, ILogger<OrderTransferTransactionTracker> logger, double intervalInMs)
    {
        _blockchain = blockchain;
        _unitOfWork = unitOfWork;
        _exchangerConfiguration = exchangerConfiguration;
        _logger = logger;
        
        _synchronizer = 0;

        _timer = new Timer { AutoReset = true, Enabled = false, Interval = intervalInMs };
        _timer.Elapsed += Handler;
        _timer.Start();
    }

    private async void Handler(object? _, ElapsedEventArgs __) => await ExecuteConcurrentlyAsync(async () =>
    {
        var updatedSellOrders = new List<Domain.Entities.SellOrder>();
        var updatedBuyOrders = new List<Domain.Entities.BuyOrder>();

        foreach (var trackedSellOrder in await _unitOfWork.Repository.GetAll<Domain.Entities.SellOrder>(o => o.Status == OrderStatus.Created))
            if (await HandleSellerToExchangerTransferTransaction(trackedSellOrder.SellerToExchangerTransferTransactionHash, trackedSellOrder))
                updatedSellOrders.Add(trackedSellOrder);
        
        foreach (var trackedSellOrder in await _unitOfWork.Repository.GetAll<Domain.Entities.SellOrder>(o => o.Status == OrderStatus.ReceiptFiatFromBuyerConfirmedBySeller))
            if (await HandleExchangerToBuyerTransferTransaction(trackedSellOrder))
                updatedSellOrders.Add(trackedSellOrder);
        
        foreach (var trackedBuyOrder in await _unitOfWork.Repository.GetAll<Domain.Entities.BuyOrder>(o => o.Status == OrderStatus.RespondedBySeller))
            if (await HandleSellerToExchangerTransferTransaction(trackedBuyOrder.SellerToExchangerTransferTransactionHash!, trackedBuyOrder))
                updatedBuyOrders.Add(trackedBuyOrder);
        
        foreach (var trackedBuyOrder in await _unitOfWork.Repository.GetAll<Domain.Entities.BuyOrder>(o => o.Status == OrderStatus.ReceiptFiatFromBuyerConfirmedBySeller))
            if (await HandleExchangerToBuyerTransferTransaction(trackedBuyOrder))
                updatedBuyOrders.Add(trackedBuyOrder);
        
        if (updatedSellOrders.Count > 0)
            _unitOfWork.Repository.UpdateRange(updatedSellOrders);
        if (updatedBuyOrders.Count > 0)
            _unitOfWork.Repository.UpdateRange(updatedBuyOrders);
        if (updatedBuyOrders.Count > 0 || updatedSellOrders.Count > 0)
        {
            await _unitOfWork.SaveAllTrackedEntities();
            _unitOfWork.UntrackAllEntities();
        }
    });
    
    private async Task ExecuteConcurrentlyAsync(Func<Task> action)
    {
        Interlocked.Increment(ref _synchronizer);

        while (_synchronizer != 1)
            await Task.Yield();

        await action();

        Interlocked.Decrement(ref _synchronizer);
    }

    private async Task<bool> HandleSellerToExchangerTransferTransaction(string transactionHash, BaseOrder trackedOrder)
    {
        var transaction = await _blockchain.TryGetTransactionByHash(transactionHash);

        if (transaction == null)
        {
            _logger.LogInformation(
                "OrderTransferTransactionTracker: transaction {TransactionHash} does not exist. Order {OrderGuid}.",
                transactionHash, trackedOrder.Guid);

            trackedOrder.Cancel();
            
            return true;
        }

        if (transaction.Status == TransferTransactionStatus.Rejected)
        {
            _logger.LogCritical(
                "OrderTransferTransactionTracker: transaction {TransactionHash} is rejected. Order {OrderGuid}.",
                transactionHash, trackedOrder.Guid);

            trackedOrder.Cancel();
            
            return true;
        }

        if (transaction.Status == TransferTransactionStatus.InProcess)
            return false;

        if (transaction.To != _exchangerConfiguration.AccountAddress)
        {
            _logger.LogInformation(
                "OrderTransferTransactionTracker: transaction {TransactionHash} recipient is invalid. Order {OrderGuid}.",
                transactionHash, trackedOrder.Guid);

            trackedOrder.Cancel();

            return true;
        }

        var expectedCryptoAmount = trackedOrder.CryptoAmount + trackedOrder.SellerToExchangerFee + trackedOrder.ExchangerToMinersFee;

        if (expectedCryptoAmount != transaction.Amount)
        {
            var refundTransactionHash = await _blockchain.SendTransferTransaction(transaction.From, transaction.Amount - trackedOrder.ExchangerToMinersFee);

            _logger.LogInformation(
                "OrderTransferTransactionTracker: transaction {TransactionHash} amount should have been {ExpectedCryptoAmount}. Refund transaction hash: {RefundTransactionHash}. Order {OrderGuid}.",
                transactionHash, expectedCryptoAmount, refundTransactionHash, trackedOrder.Guid);

            trackedOrder.Cancel();
            
            return true;
        }

        _logger.LogInformation(
            "OrderTransferTransactionTracker: transaction {TransactionHash} is confirmed. Order {OrderGuid}.",
            transactionHash, trackedOrder.Guid);

        trackedOrder.ConfirmSellerToExchangerTransferTransaction();

        return true;
    }
    
    private async Task<bool> HandleExchangerToBuyerTransferTransaction(BaseOrder trackedOrder)
    {
        var transactionHash = trackedOrder.ExchangerToBuyerTransferTransactionHash!;

        var transaction = await _blockchain.TryGetTransactionByHash(transactionHash);
                
        if (transaction == null )
        {
            _logger.LogCritical(
                "OrderTransferTransactionTracker: transaction {TransactionHash} does not exist. Order {OrderGuid}.",
                transactionHash, trackedOrder.Guid);

            throw new Exception("Alarm!");
        }

        if (transaction.Status == TransferTransactionStatus.Rejected)
        {
            _logger.LogCritical(
                "OrderTransferTransactionTracker: transaction {TransactionHash} is rejected. Order {OrderGuid}.",
                transactionHash, trackedOrder.Guid);

            throw new Exception("Alarm!");
        }

        if (transaction.Status == TransferTransactionStatus.InProcess)
            return false;
                
        _logger.LogInformation(
            "OrderTransferTransactionTracker: transaction {TransactionHash} is confirmed. Order {OrderGuid}.",
            transactionHash, trackedOrder.Guid);

        trackedOrder.ConfirmExchangerToBuyerTransferTransaction();

        return true;
    }

    public void Dispose()
    {
        _timer.Elapsed -= Handler;
        _timer.Stop();
        
        Join();
        
        _timer.Dispose();
    }
    
    private void Join()
    {
        while (_synchronizer != 0)
            Thread.Yield();
    }
    
    private readonly IBlockchain _blockchain;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ExchangerConfiguration _exchangerConfiguration;

    private readonly ILogger<OrderTransferTransactionTracker> _logger;
    
    private int _synchronizer;

    private readonly Timer _timer;
}