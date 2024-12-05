using Core.Application.Commands;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Enums;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Core.Application.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<(Guid, OrderStatus)>>
{
    public CreateOrderHandler(IValidator<CreateOrderCommand> validator, IBlockchain blockchain, IOrderStorage orderStorage)
    {
        _validator = validator;
        _blockchain = blockchain;
        _orderStorage = orderStorage;
    }

    public async Task<Result<(Guid, OrderStatus)>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(e => e.ErrorMessage));

        var sellerToExchangerFee = request.CryptoAmount * FeeRate;
        var exchangerToMinersFee = await _blockchain.GetTransferTransactionFee();
        var order = new Order(Guid.NewGuid(), request.Crypto, request.CryptoAmount, request.Fiat,
            request.CryptoToFiatExchangeRate, request.PaymentMethodInfo, (sellerToExchangerFee, exchangerToMinersFee));
        
        await _orderStorage.Add(order);

        return Result.Ok((order.Guid, order.Status));
    }
    
    public static decimal FeeRate { get; set; }

    private readonly IValidator<CreateOrderCommand> _validator;

    private readonly IBlockchain _blockchain;

    private readonly IOrderStorage _orderStorage;
}