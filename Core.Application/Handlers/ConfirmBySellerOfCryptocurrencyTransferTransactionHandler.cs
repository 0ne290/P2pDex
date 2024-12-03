using Core.Application.Commands;
using Core.Application.Services;
using Core.Domain.Enums;
using Core.Domain.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Handlers;

public class ConfirmBySellerOfCryptocurrencyTransferTransactionHandler : IRequestHandler<ConfirmBySellerOfCryptocurrencyTransferTransactionCommand, Result<(string, OrderStatus)>>
{
    public ConfirmBySellerOfCryptocurrencyTransferTransactionHandler(ITraderStorage traderStorage,
        IOrderStorage orderStorage, IBlockchain blockchain,
        OrderTransferTransactionTracker orderTransferTransactionTracker,
        ILogger<ConfirmBySellerOfCryptocurrencyTransferTransactionHandler> logger)
    {
        _traderStorage = traderStorage;
        _orderStorage = orderStorage;
        _blockchain = blockchain;
        _orderTransferTransactionTracker = orderTransferTransactionTracker;
        _logger = logger;
    }

    public async Task<Result<(string, OrderStatus)>> Handle(ConfirmBySellerOfCryptocurrencyTransferTransactionCommand request, CancellationToken _)
    {
        var seller = await _traderStorage.TryGetByGuid(request.SellerGuid);

        if (seller == null)
            return Result.Fail("Trader does not exist.");

        var order = await _orderStorage.TryGetByGuid(request.OrderGuid);

        if (order == null)
            return Result.Fail("Order does not exist.");
        if (!order.Seller.Equals(seller))
            return Result.Fail("Trader is not the seller of the order.");
        if (order.Status != OrderStatus.WaitingConfirmBySellerOfCryptocurrencyTransferTransaction)
            return Result.Fail("Order status is invalid.");

        var transaction = await _blockchain.TryGetTransferTransactionInfo(request.TransactionHash);

        if (transaction == null)
            return Result.Fail("Transaction does not exist.");
        if (transaction.To != _blockchain.ExchangerAccountAddress)
            return Result.Fail(
                "Cryptocurrency was transferred to the wrong address. For a refund, contact the recipient.");

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

        var transactionStatus =
            await _blockchain.GetTransferTransactionStatus(request.TransactionHash);

        switch (transactionStatus)
        {
            case TransferTransactionStatus.Cancelled:
                order.Cancel();
                await _orderStorage.Update(order);

                _logger.LogWarning("Blockchain rejected the {@Transaction}.", transaction);

                return Result.Fail("Blockchain rejected the transaction. Order cancelled.");
            case TransferTransactionStatus.Confirmed:
                order.ConfirmByBlockchainOfCryptocurrencyTransferTransaction();
                await _orderStorage.Update(order);
                
                return Result.Ok((order.Guid, order.Status));
            case TransferTransactionStatus.WaitingConfirmation:
                await _orderStorage.Update(order);
                _orderTransferTransactionTracker.Track(order);
                
                return Result.Ok((order.Guid, order.Status));
            default:
                throw new Exception($"{nameof(transactionStatus)} is invalid.");
        }
    }

    private readonly ITraderStorage _traderStorage;

    private readonly IOrderStorage _orderStorage;

    private readonly IBlockchain _blockchain;

    private readonly OrderTransferTransactionTracker _orderTransferTransactionTracker;

    private readonly ILogger<ConfirmBySellerOfCryptocurrencyTransferTransactionHandler> _logger;
}
