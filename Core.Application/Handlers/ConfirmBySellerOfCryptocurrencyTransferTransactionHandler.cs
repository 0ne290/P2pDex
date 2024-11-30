using Core.Application.Commands;
using Core.Application.Services;
using Core.Domain.Enums;
using Core.Domain.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Core.Application.Handlers;

public class ConfirmBySellerOfCryptocurrencyTransferTransactionHandler
{
    public ConfirmBySellerOfCryptocurrencyTransferTransactionHandler(ITraderStorage traderStorage,
        IOrderStorage orderStorage, IBlockchain blockchain,
        OrderTransferTransactionTracker orderTransferTransactionTracker,
        ILogger<ConfirmBySellerOfCryptocurrencyTransferTransactionHandler> logger)
    {
        logger.LogDebug("{Constructor} is invoked by {Param1}, {Param2}, {Param3}, {@Param4}, {Param5}.",
            typeof(ConfirmBySellerOfCryptocurrencyTransferTransactionHandler), traderStorage.GetType(), orderStorage.GetType(), blockchain.GetType(),
            orderTransferTransactionTracker.GetType(), logger.GetType());
        
        _traderStorage = traderStorage;
        _orderStorage = orderStorage;
        _blockchain = blockchain;
        _orderTransferTransactionTracker = orderTransferTransactionTracker;
        _logger = logger;
        
        logger.LogDebug("{Constructor} is finished.", typeof(ConfirmBySellerOfCryptocurrencyTransferTransactionHandler));
    }

    public async Task<Result> Handle(ConfirmBySellerOfCryptocurrencyTransferTransactionCommand request)
    {
        var seller = await _traderStorage.TryGetByGuid(request.SellerGuid);

        if (seller == null)
            return Result.Fail($"Trader with guid \"{request.SellerGuid}\" does not exist.");

        var order = await _orderStorage.TryGetByGuid(request.OrderGuid);

        if (order == null)
            return Result.Fail($"Order with guid \"{request.OrderGuid}\" does not exist.");
        if (!order.Seller.Equals(seller))
            return Result.Fail(
                $"Trader with guid \"{seller.Guid}\" is not the seller of the order with guid \"{order.Guid}\".");

        var transaction = await _blockchain.TryGetTransferTransactionInfo(request.TransactionHash);

        if (transaction == null)
            return Result.Fail($"Transaction with hash \"{request.TransactionHash}\" does not exist.");
        if (transaction.To != _blockchain.ExchangerAccountAddress)
            return Result.Fail(
                $"Cryptocurrency was transferred to the wrong address. Order cancelled. For a refund, contact the owner of address {transaction.To}.");

        order.ConfirmBySellerOfCryptocurrencyTransferTransaction(request.TransactionHash);

        var expectedCryptoAmount =
            order.CryptoAmount + order.Fee.SellerToExchanger + order.Fee.ExpectedExchangerToMiners;

        if (expectedCryptoAmount != transaction.Amount)
        {
            var refundTransactionHash = await _blockchain.SendTransferTransaction(transaction.From,
                transaction.Amount - order.Fee.ExpectedExchangerToMiners);
            order.Cancel();
            await _orderStorage.Update(order);

            return Result.Fail(
                $"Amount of cryptocurrency transferred should have been {expectedCryptoAmount}. Order cancelled. Cryptocurrency refund transaction with the collected transfer fee has already been accepted for processing. Wait for confirmation by blockchain. Refund transaction hash: {refundTransactionHash}.");
        }

        var transactionStatus = await _blockchain.GetTransferTransactionStatus(request.TransactionHash);

        if (transactionStatus == TransferTransactionStatus.Cancelled)
        {
            order.Cancel();
            await _orderStorage.Update(order);

            return Result.Fail($"Blockchain rejected the transaction. Order cancelled.");
        }

        if (transactionStatus == TransferTransactionStatus.Confirmed)
        {
            order.ConfirmByBlockchainOfCryptocurrencyTransferTransaction();
            await _orderStorage.Update(order);
        }
        else
        {
            await _orderStorage.Update(order);
            _orderTransferTransactionTracker.Track(order);
        }

        return Result.Ok();
    }

    private readonly ITraderStorage _traderStorage;

    private readonly IOrderStorage _orderStorage;

    private readonly IBlockchain _blockchain;

    private readonly OrderTransferTransactionTracker _orderTransferTransactionTracker;

    private readonly ILogger<ConfirmBySellerOfCryptocurrencyTransferTransactionHandler> _logger;
}