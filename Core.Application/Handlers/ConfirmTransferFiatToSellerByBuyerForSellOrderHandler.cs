using Core.Application.Commands;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.Handlers;

public class ConfirmTransferFiatToSellerByBuyerForSellOrderHandler : IRequestHandler<ConfirmOrderByBuyerCommand, CommandResult>
{
    public ConfirmTransferFiatToSellerByBuyerForSellOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<CommandResult> Handle(ConfirmOrderByBuyerCommand request, CancellationToken _)
    {
        var order = await _unitOfWork.Repository.TryGetByGuid<SellOrder>(request.OrderGuid);

        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        
        order.ConfirmTransferFiatToSellerByBuyer();

        if (!Equals(order.BuyerGuid, request.BuyerGuid))
            throw new InvariantViolationException("Trader is not a buyer.");

        await _unitOfWork.SaveAllTrackedEntities();
        
        return new CommandResult(new { guid = order.Guid, status = order.Status.ToString() });
    }

    private readonly IUnitOfWork _unitOfWork;
}