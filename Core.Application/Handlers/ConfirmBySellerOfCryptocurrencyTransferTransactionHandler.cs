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
        
        // if (order.CryptoAmount + order.FeeFromSeller != transaction.CryptoAmount)
        // Перевести transaction.CryptoAmount - order.Fee.ExpectedFeeExchangerToMiners обратно продавцу и отменить заказ
        
        // Проверить статус транзакции. Если подтверждена - перевести заказ в следующую стадию. Если нет - поставить
        // транзакцию на отслеживание

        return Result.Ok();

    }

    private readonly ITraderStorage _traderStorage;
    
    private readonly IOrderStorage _orderStorage;

    private readonly IBlockchain _blockchain;
}