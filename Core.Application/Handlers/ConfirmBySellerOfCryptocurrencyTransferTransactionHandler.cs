using Core.Application.Commands;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using FluentResults;
using FluentValidation;

namespace Core.Application.Handlers;

public class ConfirmBySellerOfCryptocurrencyTransferTransactionHandler
{
    public ConfirmBySellerOfCryptocurrencyTransferTransactionHandler(ITraderStorage traderStorage, IOrderStorage orderStorage, IBlockchain blockchain)
    {
        _traderStorage = traderStorage;
        _orderStorage = orderStorage;
        _blockchain = blockchain;
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
            return Result.Fail($"Trader with guid \"{seller.Guid}\" is not the seller of the order with guid \"{order.Guid}\".");

        var transaction = await _blockchain.TryGetTransactionByHash(request.TransactionHash);
        if (transaction == null)
            return Result.Fail($"Transaction with hash \"{request.TransactionHash}\" does not exist.");

        var expectedCryptoAmount =
            order.CryptoAmount + order.Fee.SellerToExchanger + order.Fee.ExpectedExchangerToMiners;
        if (expectedCryptoAmount != transaction.CryptoAmount)
        {
            await _blockchain.SendTransaction(transaction.From, transaction.To, transaction.CryptoAmount - order.Fee.ExpectedExchangerToMiners);
            order.Cancel();
            await _orderStorage.Update(order);
            return Result.Fail($"Amount of cryptocurrency transferred should have been {expectedCryptoAmount}. Order cancelled. Cryptocurrency refund transaction with the collected transfer fee has already been accepted for processing. Wait for confirmation by blockchain.");
        }
        
        // Проверить статус транзакции. Если подтверждена - перевести заказ в следующую стадию. Если нет - поставить
        // транзакцию на отслеживание

        return Result.Ok();

    }

    private readonly ITraderStorage _traderStorage;
    
    private readonly IOrderStorage _orderStorage;

    private readonly IBlockchain _blockchain;
}