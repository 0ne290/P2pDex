using Core.Application.Commands;
using Core.Application.Services;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Core.Application.Handlers;

public class CreateSellOrderHandler
{
    public CreateSellOrderHandler(IValidator<CreateSellOrderCommand> validator, ITraderStorage traderStorage,
        FeeCalculator feeCalculator, IOrderStorage orderStorage, ILogger<CreateSellOrderHandler> logger)
    {
        logger.LogDebug("{Constructor} is invoked by {Param1}, {Param2}, {Param3}, {Param4}, {Param5}.",
            typeof(CreateSellOrderHandler), validator.GetType(), traderStorage.GetType(), feeCalculator.GetType(),
            orderStorage.GetType(), logger.GetType());

        _validator = validator;
        _traderStorage = traderStorage;
        _feeCalculator = feeCalculator;
        _orderStorage = orderStorage;
        _logger = logger;

        logger.LogDebug("{Constructor} is finished.", typeof(CreateSellOrderHandler));
    }

    public async Task<Result> Handle(CreateSellOrderCommand request)
    {
        _logger.LogDebug("{Method} is invoked by {@Param1}.",
            $"{typeof(CreateSellOrderHandler)}.{nameof(Handle)}", request);
        
        var validationResult = await _validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
            return Result.Fail(validationResult.ToString());

        var seller = await _traderStorage.TryGetByGuid(request.SellerGuid);
        
        if (seller == null)
            return Result.Fail($"Trader does not exist.");

        var feeFromSeller = await _feeCalculator.Calculate(request.CryptoAmount);
        var order = new SellOrder(Guid.NewGuid().ToString(), request.Crypto, request.CryptoAmount, request.Fiat,
            request.CryptoToFiatExchangeRate, request.PaymentMethodInfo, seller, feeFromSeller);
        
        await _orderStorage.Add(order);
        
        _logger.LogDebug("{Method} is returned {@Result}.",
            $"{typeof(CreateSellOrderHandler)}.{nameof(Handle)}", result);

        return Result.Ok();
    }

    private readonly IValidator<CreateSellOrderCommand> _validator;

    private readonly ITraderStorage _traderStorage;

    private readonly FeeCalculator _feeCalculator;

    private readonly IOrderStorage _orderStorage;

    private readonly ILogger<CreateSellOrderHandler> _logger;
}
