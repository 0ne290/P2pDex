using Core.Application.Commands;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Interfaces;
using Core.Domain.Services;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Core.Application.Handlers;

public class CreateSellOrderHandler : IRequestHandler<CreateSellOrderCommand, Result<(Guid, OrderStatus)>>
{
    public CreateSellOrderHandler(IOrderStorage orderStorage, Exchanger exchanger)
    {
        _validator = validator;
        _blockchain = blockchain;
        _orderStorage = orderStorage;
    }

    public async Task<Result<(Guid, OrderStatus)>> Handle(CreateSellOrderCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(e => e.ErrorMessage));

        var sellerToExchangerFee = request.CryptoAmount * FeeRate;
        var exchangerToMinersFee = await _blockchain.GetTransferTransactionFee();
        var order = new SellOrder(Guid.NewGuid(), request.Crypto, request.CryptoAmount, request.Fiat,
            request.CryptoToFiatExchangeRate, request.PaymentMethodInfo, (sellerToExchangerFee, exchangerToMinersFee));
        
        await _orderStorage.Add(order);

        return Result.Ok((order.Guid, Status: order.CurrentStatus));
    }
    
    private readonly IOrderStorage _orderStorage;
    
    private readonly Exchanger
}