using Core.Application.Api.SellOrder.Commands;
using Core.Application.Private.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;

namespace Core.Application.Api.SellOrder.Handlers;

public class ConfirmTransferFiatToSellerByBuyerForSellOrderHandler : IRequestHandler<ConfirmTransferFiatToSellerByBuyerForSellOrderCommand, OrderStatusChangeResponse>
{
    public ConfirmTransferFiatToSellerByBuyerForSellOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<OrderStatusChangeResponse> Handle(ConfirmTransferFiatToSellerByBuyerForSellOrderCommand request, CancellationToken _)
    {
        if (!await _unitOfWork.Repository.Exists<Trader>(t => t.Id == request.BuyerId))
            throw new InvariantViolationException("Buyer does not exists.");
        
        var order = await _unitOfWork.Repository.TryGet<Domain.Entities.SellOrder>(o => o.Guid.Equals(request.OrderGuid));
        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        
        order.ConfirmTransferFiatToSellerByBuyer();

        if (!Equals(order.BuyerId, request.BuyerId))
            throw new InvariantViolationException("Trader is not a buyer.");

        await _unitOfWork.SaveAllTrackedEntities();
        
        return new OrderStatusChangeResponse { Guid = order.Guid, Status = order.Status };
    }

    private readonly IUnitOfWork _unitOfWork;
}