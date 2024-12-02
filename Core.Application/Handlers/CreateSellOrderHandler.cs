using Core.Application.Commands;
using Core.Application.Services;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Interfaces;
using FluentResults;
using FluentValidation;

namespace Core.Application.Handlers;

public class CreateSellOrderHandler
{
    public CreateSellOrderHandler(IValidator<CreateSellOrderCommand> validator, ITraderStorage traderStorage,
        FeeCalculator feeCalculator, IOrderStorage orderStorage)
    {
        _validator = validator;
        _traderStorage = traderStorage;
        _feeCalculator = feeCalculator;
        _orderStorage = orderStorage;
    }

    public async Task<Result<(string, OrderStatus)>> Handle(CreateSellOrderCommand request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
            return Result.Fail(validationResult.ToString());

        var seller = await _traderStorage.TryGetByGuid(request.SellerGuid);
        
        if (seller == null)
            return Result.Fail("Trader does not exist.");

        var feeFromSeller = await _feeCalculator.Calculate(request.CryptoAmount);
        var order = new SellOrder(Guid.NewGuid().ToString(), request.Crypto, request.CryptoAmount, request.Fiat,
            request.CryptoToFiatExchangeRate, request.PaymentMethodInfo, seller, feeFromSeller);
        
        await _orderStorage.Add(order);

        return Result.Ok((order.Guid, order.Status));
    }

    private readonly IValidator<CreateSellOrderCommand> _validator;

    private readonly ITraderStorage _traderStorage;

    private readonly FeeCalculator _feeCalculator;

    private readonly IOrderStorage _orderStorage;
}
