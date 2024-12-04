using Core.Application.Commands;
using Core.Application.Services;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Interfaces;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Core.Application.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<(Guid, OrderStatus)>>
{
    public CreateOrderHandler(IValidator<CreateOrderCommand> validator, FeeCalculator feeCalculator,
        IOrderStorage orderStorage)
    {
        _validator = validator;
        _feeCalculator = feeCalculator;
        _orderStorage = orderStorage;
    }

    public async Task<Result<(Guid, OrderStatus)>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(e => e.ErrorMessage));

        var fee = await _feeCalculator.Calculate(request.CryptoAmount);
        var order = new Order(Guid.NewGuid(), request.Crypto, request.CryptoAmount, request.Fiat,
            request.CryptoToFiatExchangeRate, request.PaymentMethodInfo, fee);
        
        await _orderStorage.Add(order);

        return Result.Ok((order.Guid, order.Status));
    }

    private readonly IValidator<CreateOrderCommand> _validator;

    private readonly FeeCalculator _feeCalculator;

    private readonly IOrderStorage _orderStorage;
}
