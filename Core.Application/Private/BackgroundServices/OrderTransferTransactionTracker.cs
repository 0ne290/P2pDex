using Core.Application.Private.Configurations;
using Core.Application.Private.Constants;
using Core.Application.Private.Interfaces;
using Core.Domain.Constants;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Application.Private.BackgroundServices;

public class OrderTransferTransactionTracker : BackgroundService
{
    public OrderTransferTransactionTracker(IBlockchain blockchain, IUnitOfWork unitOfWork,
        ExchangerConfiguration exchangerConfiguration, ILogger<OrderTransferTransactionTracker> logger, int intervalInMs)
    {
        _blockchain = blockchain;
        _unitOfWork = unitOfWork;
        _exchangerConfiguration = exchangerConfiguration;
        _logger = logger;
        _intervalInMs = intervalInMs;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("OrderTransferTransactionTracker: iteration started.");
            
            _updatedSellOrders = [];

            await HandleSellerToExchangerTransferTransaction();
            await HandleExchangerToBuyerTransferTransaction();
            await UpdateSellOrders();
            
            _logger.LogInformation("OrderTransferTransactionTracker: iteration completed.");
 
            await Task.Delay(_intervalInMs, stoppingToken);
        }
    }
    
    private async Task HandleSellerToExchangerTransferTransaction()
    {
        foreach (var trackedSellOrder in await _unitOfWork.Repository.GetAllBy<Domain.Entities.SellOrder>(o => o.Status == OrderStatus.Created))
        {
            var transactionHash = trackedSellOrder.SellerToExchangerTransferTransactionHash;
            
            var transaction = await _blockchain.TryGetTransactionByHash(transactionHash);
            if (transaction == null)
            {
                _logger.LogInformation(
                    "OrderTransferTransactionTracker: transaction {TransactionHash} does not exist. Order {OrderGuid}.",
                    transactionHash, trackedSellOrder.Guid);

                trackedSellOrder.Cancel();
                _updatedSellOrders.Add(trackedSellOrder);

                continue;
            }

            if (transaction.Status == TransferTransactionStatus.Rejected)
            {
                _logger.LogCritical(
                    "OrderTransferTransactionTracker: transaction {TransactionHash} is rejected. Order {OrderGuid}.",
                    transactionHash, trackedSellOrder.Guid);

                trackedSellOrder.Cancel();
                _updatedSellOrders.Add(trackedSellOrder);

                continue;
            }
            if (transaction.Status == TransferTransactionStatus.InProcess)
                continue;

            if (transaction.To != _exchangerConfiguration.AccountAddress)
            {
                _logger.LogInformation(
                    "OrderTransferTransactionTracker: transaction {TransactionHash} recipient is invalid. Order {OrderGuid}.",
                    transactionHash, trackedSellOrder.Guid);

                trackedSellOrder.Cancel();
                _updatedSellOrders.Add(trackedSellOrder);

                continue;
            }

            var expectedCryptoAmount = trackedSellOrder.CryptoAmount + trackedSellOrder.SellerToExchangerFee +
                                       trackedSellOrder.ExchangerToMinersFee;
            if (expectedCryptoAmount != transaction.Amount)
            {
                var refundTransactionHash = await _blockchain.SendTransferTransaction(transaction.From,
                    transaction.Amount - trackedSellOrder.ExchangerToMinersFee);

                _logger.LogInformation(
                    "OrderTransferTransactionTracker: transaction {TransactionHash} amount should have been {ExpectedCryptoAmount}. Refund transaction hash: {RefundTransactionHash}. Order {OrderGuid}.",
                    transactionHash, expectedCryptoAmount, refundTransactionHash, trackedSellOrder.Guid);

                trackedSellOrder.Cancel();
                _updatedSellOrders.Add(trackedSellOrder);

                continue;
            }

            _logger.LogInformation(
                "OrderTransferTransactionTracker: transaction {TransactionHash} is confirmed. Order {OrderGuid}.",
                transactionHash, trackedSellOrder.Guid);

            trackedSellOrder.ConfirmSellerToExchangerTransferTransaction();
            _updatedSellOrders.Add(trackedSellOrder);
        }
    }
    
    private async Task HandleExchangerToBuyerTransferTransaction()
    {
        foreach (var trackedSellOrder in await _unitOfWork.Repository.GetAllBy<Domain.Entities.SellOrder>(o =>
                     o.Status == OrderStatus.ReceiptFiatFromBuyerConfirmedBySeller))
        {
            var transactionHash = trackedSellOrder.ExchangerToBuyerTransferTransactionHash!;

            var transaction = await _blockchain.TryGetTransactionByHash(transactionHash);
            if (transaction == null )
            {
                _logger.LogCritical(
                    "OrderTransferTransactionTracker: transaction {TransactionHash} does not exist. Order {OrderGuid}.",
                    transactionHash, trackedSellOrder.Guid);

                throw new Exception("Alarm!");
            }

            if (transaction.Status == TransferTransactionStatus.Rejected)
            {
                _logger.LogCritical(
                    "OrderTransferTransactionTracker: transaction {TransactionHash} is rejected. Order {OrderGuid}.",
                    transactionHash, trackedSellOrder.Guid);

                throw new Exception("Alarm!");
            }
            if (transaction.Status == TransferTransactionStatus.InProcess)
                continue;
                
            _logger.LogInformation(
                "OrderTransferTransactionTracker: transaction {TransactionHash} is confirmed. Order {OrderGuid}.",
                transactionHash, trackedSellOrder.Guid);

            trackedSellOrder.ConfirmExchangerToBuyerTransferTransaction();
            _updatedSellOrders.Add(trackedSellOrder);
        }
    }

    private async Task UpdateSellOrders()
    {
        if (_updatedSellOrders.Count > 0)
        {
            _unitOfWork.Repository.UpdateRange(_updatedSellOrders);
            await _unitOfWork.SaveAllTrackedEntities();
            _unitOfWork.UntrackAllEntities();
        }
    }

    private List<Domain.Entities.SellOrder> _updatedSellOrders = null!;
    
    private readonly IBlockchain _blockchain;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ExchangerConfiguration _exchangerConfiguration;

    private readonly ILogger<OrderTransferTransactionTracker> _logger;

    private readonly int _intervalInMs;
}