using Core.Application.Commands;
using Core.Domain.Interfaces;
using Core.Domain.Services;
using MediatR;

namespace Core.Application.Handlers;

public class CreateSellOrderHandler : IRequestHandler<CreateSellOrderCommand, CommandResult>
{
    public CreateSellOrderHandler(IUnitOfWork unitOfWork, Exchanger exchanger)
    {
        _unitOfWork = unitOfWork;
        _exchanger = exchanger;
    }

    public async Task<CommandResult> Handle(CreateSellOrderCommand request, CancellationToken _)
    {
        var order = await _exchanger.CreateSellOrder(request.Crypto, request.CryptoAmount, request.Fiat,
            request.CryptoToFiatExchangeRate, request.PaymentMethodInfo, request.SellerGuid,
            request.TransferTransactionHash);

        await _unitOfWork.Repository.Add(order);
        await _unitOfWork.Save();
        
        return new CommandResult(new { guid = order.Guid, status = order.Status });
    }
    
    private readonly IUnitOfWork _unitOfWork;

    private readonly Exchanger _exchanger;
}