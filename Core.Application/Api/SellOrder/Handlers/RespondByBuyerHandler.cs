using Core.Application.Api.SellOrder.Commands;
using Core.Application.Private.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;

namespace Core.Application.Api.SellOrder.Handlers;

public class RespondToSellOrderByBuyerHandler : IRequestHandler<RespondToSellOrderByBuyerCommand, OrderStatusChangeResponse>
{
    public RespondToSellOrderByBuyerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<OrderStatusChangeResponse> Handle(RespondToSellOrderByBuyerCommand request, CancellationToken _)
    {
        var order = await _unitOfWork.Repository.TryGet<Domain.Entities.SellOrder>(o => o.Guid.Equals(request.OrderGuid));
        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        
        order.RespondByBuyer(request.BuyerId, request.BuyerAccountAddress);
        
        if (!await _unitOfWork.Repository.Exists<Trader>(t => t.Id == request.BuyerId))
            throw new InvariantViolationException("Buyer does not exists.");
    
        await _unitOfWork.SaveAllTrackedEntities();

        return new OrderStatusChangeResponse { Guid = order.Guid, Status = order.Status };
    }

    private readonly IUnitOfWork _unitOfWork;
}
