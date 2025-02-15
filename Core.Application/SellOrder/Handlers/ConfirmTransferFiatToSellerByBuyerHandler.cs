using Core.Application.SellOrder.Commands;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.SellOrder.Handlers;

public class ConfirmTransferFiatToSellerByBuyerForSellOrderHandler : IRequestHandler<ConfirmTransferFiatToSellerByBuyerForSellOrderCommand, CommandResult>
{
    public ConfirmTransferFiatToSellerByBuyerForSellOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<CommandResult> Handle(ConfirmTransferFiatToSellerByBuyerForSellOrderCommand request, CancellationToken _)
    {
        var order = await _unitOfWork.Repository.TryGet<Domain.Entities.SellOrder>(o => o.Guid.Equals(request.OrderGuid));

        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        
        order.ConfirmTransferFiatToSellerByBuyer();

        if (!Equals(order.BuyerId, request.BuyerId))
            throw new InvariantViolationException("Trader is not a buyer.");

        await _unitOfWork.SaveAllTrackedEntities();
        
        var ret = new Dictionary<string, object>
        {
            ["guid"] = order.Guid,
            ["status"] = order.Status.ToString()
        };
        
        return new CommandResult(ret);
    }

    private readonly IUnitOfWork _unitOfWork;
}