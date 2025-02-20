using Core.Application.Private.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;

namespace Core.Application.Api.SellOrder.Get;

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
        
        var order = await _unitOfWork.SellOrderAndItsTradersQuery.Execute(o => o.Guid.Equals(request.OrderGuid));
        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        
        if (order.BuyerId != null && order.SellerId != request.TraderId && order.BuyerId != request.TraderId)
            throw new InvariantViolationException("Trader is not a buyer.");
        
        IDictionary<string, object> ret = new Dictionary<string, object>
        {
            ["sellOrder"] = order
        };
        
        return ret;
    }
    
    private readonly IUnitOfWork _unitOfWork;
}