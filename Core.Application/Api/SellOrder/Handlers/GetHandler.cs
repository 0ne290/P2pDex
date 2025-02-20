using Core.Application.Api.SellOrder.Commands;
using Core.Application.Private.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;

namespace Core.Application.Api.SellOrder.Handlers;

public class GetSellOrderHandler : IRequestHandler<GetSellOrderCommand, IDictionary<string, object>>
{
    public GetSellOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IDictionary<string, object>> Handle(GetSellOrderCommand request, CancellationToken _)
    {
        if (!await _unitOfWork.Repository.Exists<Trader>(t => t.Id == request.TraderId))
            throw new InvariantViolationException("Trader does not exists.");
        
        dynamic? order = await _unitOfWork.SellOrderAndItsTradersQuery.Execute(o => o.Guid.Equals(request.OrderGuid));
        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        
        if (order.buyerId != null && order.sellerId != request.TraderId && order.buyerId != request.TraderId)
            throw new InvariantViolationException("Trader is not a buyer.");
        
        IDictionary<string, object> ret = new Dictionary<string, object>
        {
            ["sellOrder"] = order
        };
        
        return ret;
    }
    
    private readonly IUnitOfWork _unitOfWork;
}