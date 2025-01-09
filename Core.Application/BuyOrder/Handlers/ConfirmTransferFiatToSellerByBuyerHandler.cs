using Core.Application.BuyOrder.Commands;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.BuyOrder.Handlers;

public class ConfirmTransferFiatToSellerByBuyerForBuyOrderHandler : IRequestHandler<ConfirmTransferFiatToSellerByBuyerForBuyOrderCommand, CommandResult>
{
    public ConfirmTransferFiatToSellerByBuyerForBuyOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<CommandResult> Handle(ConfirmTransferFiatToSellerByBuyerForBuyOrderCommand request, CancellationToken _)
    {
        var order = await _unitOfWork.Repository.TryGetByGuid<Domain.Entities.BuyOrder>(request.OrderGuid);

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