using Core.Application.Commands;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Interfaces;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Core.Application.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateSellOrderCommand, Result<(Guid, OrderStatus)>>
{
    public CreateOrderHandler(IValidator<CreateSellOrderCommand> validator, IBlockchain blockchain, IOrderStorage orderStorage)
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
    
    public static decimal FeeRate { get; set; }

    private readonly IValidator<CreateSellOrderCommand> _validator;

    private readonly IBlockchain _blockchain;

    private readonly IOrderStorage _orderStorage;
}