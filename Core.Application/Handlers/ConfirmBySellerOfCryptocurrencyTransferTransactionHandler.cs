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
        _logger.LogDebug("{Method} is invoked by {@Param1}.",
            $"{typeof(ConfirmBySellerOfCryptocurrencyTransferTransactionHandler)}.{nameof(Handle)}", request);

        Result result;
        var seller = await _traderStorage.TryGetByGuid(request.SellerGuid);

        if (seller != null)
        {
            var order = await _orderStorage.TryGetByGuid(request.OrderGuid);

            if (order != null)
            {
                if (order.Seller.Equals(seller))
                {
                    var transaction = await _blockchain.TryGetTransferTransactionInfo(request.TransactionHash);

                    if (transaction != null)
                    {
                        if (transaction.To == _blockchain.ExchangerAccountAddress)
                        {
                            order.ConfirmBySellerOfCryptocurrencyTransferTransaction(request.TransactionHash);

                            var expectedCryptoAmount =
                                order.CryptoAmount + order.Fee.SellerToExchanger + order.Fee.ExpectedExchangerToMiners;

                            if (expectedCryptoAmount == transaction.Amount)
                            {
                                var transactionStatus =
                                    await _blockchain.GetTransferTransactionStatus(request.TransactionHash);

                                if (transactionStatus != TransferTransactionStatus.Cancelled)
                                {
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

                                    result = Result.Ok();
                                }
                                else
                                {
                                    order.Cancel();
                                    await _orderStorage.Update(order);

                                    _logger.LogWarning("Blockchain rejected the {@Transaction}.", transaction);

                                    result = Result.Fail("Blockchain rejected the transaction. Order cancelled.");
                                }
                            }
                            else
                            {
                                var refundTransactionHash = await _blockchain.SendTransferTransaction(transaction.From,
                                    transaction.Amount - order.Fee.ExpectedExchangerToMiners);

                                order.Cancel();
                                await _orderStorage.Update(order);

                                result = Result.Fail(
                                    $"Amount of cryptocurrency transferred should have been {expectedCryptoAmount}. Order cancelled. Cryptocurrency refund transaction with the collected transfer fee has already been accepted for processing. Wait for confirmation by blockchain. Refund transaction hash: {refundTransactionHash}.");
                            }
                        }
                        else
                            result = Result.Fail(
                                $"Cryptocurrency was transferred to the wrong address. For a refund, contact the owner of address {transaction.To}.");
                    }
                    else
                        result = Result.Fail("Transaction does not exist.");
                }
                else
                    result = Result.Fail("Trader is not the seller of the order.");
            }
            else
                result = Result.Fail("Order does not exist.");
        }
        else
            result = Result.Fail("Trader does not exist.");

        _logger.LogDebug("{Method} is returned {@Result}.",
            $"{typeof(ConfirmBySellerOfCryptocurrencyTransferTransactionHandler)}.{nameof(Handle)}", result);

        return result;
    }

    private readonly ITraderStorage _traderStorage;

    private readonly IOrderStorage _orderStorage;

    private readonly IBlockchain _blockchain;

    private readonly OrderTransferTransactionTracker _orderTransferTransactionTracker;

    private readonly ILogger<ConfirmBySellerOfCryptocurrencyTransferTransactionHandler> _logger;
}