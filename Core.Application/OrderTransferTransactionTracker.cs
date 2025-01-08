using System.Timers;
using Core.Domain.Constants;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
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
        var updatedOrders = new List<SellOrder>();

        var trackedOrders = await _unitOfWork.Repository.GetAll<SellOrder>(o =>
            o.Status == OrderStatus.Created || o.Status == OrderStatus.ReceiptFiatFromBuyerConfirmedBySeller);
        foreach (var trackedOrder in trackedOrders)
        {
            if (trackedOrder.Status == OrderStatus.Created)
            {
                var transactionHash = trackedOrder.SellerToExchangerTransferTransactionHash;

                var transaction = await _blockchain.TryGetTransactionByHash(transactionHash);

                if (transaction == null )
                {
                    _logger.LogInformation(
                        "OrderTransferTransactionTracker: transaction {TransactionHash} does not exist. Order {OrderGuid} is canceled.",
                        transactionHash, trackedOrder.Guid);

                    trackedOrder.Cancel();
                    updatedOrders.Add(trackedOrder);
                    continue;
                }

                if (transaction.Status == TransferTransactionStatus.Rejected)
                {
                    _logger.LogCritical(
                        "OrderTransferTransactionTracker: transaction {TransactionHash} is rejected. Order {OrderGuid} is canceled.",
                        transactionHash, trackedOrder.Guid);

                    trackedOrder.Cancel();
                    updatedOrders.Add(trackedOrder);
                    continue;
                }
                
                if (transaction.Status == TransferTransactionStatus.InProcess)
                    continue;
                
                if (transaction.To != _exchangerConfiguration.AccountAddress)
                {
                    _logger.LogInformation(
                        "OrderTransferTransactionTracker: transaction {TransactionHash} recipient is invalid. Order {OrderGuid} is canceled.",
                        transactionHash, trackedOrder.Guid);

                    trackedOrder.Cancel();
                    updatedOrders.Add(trackedOrder);
                    continue;
                }

                var expectedCryptoAmount = trackedOrder.CryptoAmount + trackedOrder.SellerToExchangerFee +
                                           trackedOrder.ExchangerToMinersFee;

                if (expectedCryptoAmount != transaction.Amount)
                {
                    var refundTransactionHash = await _blockchain.SendTransferTransaction(transaction.From,
                        transaction.Amount - trackedOrder.ExchangerToMinersFee);

                    _logger.LogInformation(
                        "OrderTransferTransactionTracker: transaction {TransactionHash} amount should have been {ExpectedCryptoAmount}. Refund transaction hash: {RefundTransactionHash}. Order {OrderGuid} is canceled.",
                        transactionHash, expectedCryptoAmount, refundTransactionHash, trackedOrder.Guid);

                    trackedOrder.Cancel();
                    updatedOrders.Add(trackedOrder);
                    continue;
                }

                _logger.LogInformation(
                    "OrderTransferTransactionTracker: transaction {TransactionHash} is confirmed. Order {OrderGuid} is awaiting buyer respond.",
                    transactionHash, trackedOrder.Guid);

                updatedOrders.Add(trackedOrder);
                trackedOrder.ConfirmSellerToExchangerTransferTransaction();
            }
        }
        
        if (updatedOrders.Count > 0)
        {
            _unitOfWork.Repository.UpdateRange(updatedOrders);
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