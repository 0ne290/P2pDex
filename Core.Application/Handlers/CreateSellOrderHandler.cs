using Core.Application.Commands;
using Core.Domain.Enums;
using Core.Domain.Interfaces;
using Core.Domain.Services;
using FluentResults;
using MediatR;

namespace Core.Application.Handlers;

public class CreateSellOrderHandler : IRequestHandler<CreateSellOrderCommand, Result<(Guid, OrderStatus)>>
{
    public CreateSellOrderHandler(IUnitOfWork unitOfWork, Exchanger exchanger)
    {
        _unitOfWork = unitOfWork;
        _exchanger = exchanger;
    }

    public async Task<Result<(Guid, OrderStatus)>> Handle(CreateSellOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _exchanger.CreateSellOrder(request.Crypto, request.CryptoAmount, request.Fiat,
            request.CryptoToFiatExchangeRate, request.PaymentMethodInfo, request.SellerGuid,
            request.TransferTransactionHash);

        await _unitOfWork.Repository.Add(order);
        await _unitOfWork.Save();
        
        return Result.Ok((order.Guid, order.Status));
    }
    
    private readonly IUnitOfWork _unitOfWork;

    private readonly Exchanger _exchanger;
}