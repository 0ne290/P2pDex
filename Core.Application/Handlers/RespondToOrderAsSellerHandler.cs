using Core.Application.Commands;
using Core.Application.Enums;
using Core.Application.Interfaces;
using Core.Domain.Enums;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Handlers;

public class RespondToOrderAsSellerHandler : IRequestHandler<RespondToOrderAsSellerCommand, Result<(Guid, OrderStatus)>>
{
    public RespondToOrderAsSellerHandler(ITraderStorage traderStorage,
        IOrderStorage orderStorage, IBlockchain blockchain,
        ILogger<RespondToOrderAsSellerHandler> logger)
    {
        _traderStorage = traderStorage;
        _orderStorage = orderStorage;
        _blockchain = blockchain;
        _logger = logger;
    }

    public async Task<Result<(Guid, OrderStatus)>> Handle(RespondToOrderAsSellerCommand request, CancellationToken _)
    {
        var seller = await _traderStorage.TryGetByGuid(request.SellerGuid);

        if (seller == null)
            return Result.Fail("Trader does not exist.");

        var order = await _orderStorage.TryGetByGuid(request.OrderGuid);

        if (order == null)
            return Result.Fail("Order does not exist.");
        if (order.Status is not OrderStatus.Created and OrderStatus.BuyerResponded)
            return Result.Fail("Order status is invalid.");

        var transactionInfo = await _blockchain.TryGetTransferTransactionInfo(request.TransferTransactionHash);

        if (transactionInfo == null)
            return Result.Fail("Transaction does not exist.");
        if (transactionInfo.Status == TransferTransactionStatus.WaitingConfirmation)
            return Result.Fail("Transaction is awaiting confirmation.");
        if (transactionInfo.Status == TransferTransactionStatus.Rejected)
        {
            _logger.LogWarning("Blockchain rejected the {@Transaction}.", transactionInfo);
            
            return Result.Fail("Transaction is rejected.");
        }
        if (transactionInfo.To != _blockchain.ExchangerAccountAddress)
            return Result.Fail(
                "Cryptocurrency was transferred to the wrong address. For a refund, contact the recipient.");

        var expectedCryptoAmount =
            order.CryptoAmount + order.Fee.SellerToExchanger + order.Fee.ExchangerToMiners;

        if (expectedCryptoAmount != transactionInfo.Amount)
        {
            var refundTransactionHash = await _blockchain.SendTransferTransaction(transactionInfo.From,
                transactionInfo.Amount - order.Fee.ExchangerToMiners);

            return Result.Fail(
                $"Amount of cryptocurrency transferred should have been {expectedCryptoAmount}. Cryptocurrency refund transaction with the collected transfer fee has already been accepted for processing. Wait for confirmation by blockchain. Refund transaction hash: {refundTransactionHash}.");
        }

        order.SellerRespond(seller, request.TransferTransactionHash);
        await _orderStorage.Update(order);
        
        return Result.Ok((order.Guid, order.Status));
    }

    private readonly ITraderStorage _traderStorage;

    private readonly IOrderStorage _orderStorage;

    private readonly IBlockchain _blockchain;

    private readonly ILogger<RespondToOrderAsSellerHandler> _logger;
}