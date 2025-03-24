using Core.Application.Private.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;

namespace Core.Application.Api.SellOrder.Get;

public class GetSellOrderHandler : IRequestHandler<GetSellOrderCommand, GetSellOrderResponse>
{
    public GetSellOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetSellOrderResponse> Handle(GetSellOrderCommand request, CancellationToken _)
    {
        if (!await _unitOfWork.Repository.Exists<Trader>(t => t.Id == request.TraderId))
            throw new InvariantViolationException("Trader does not exists.");
        
        var order = await _unitOfWork.SellOrderWithTradersQuery.Execute(request.OrderGuid);
        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        
        if (order.BuyerId != null && order.SellerId != request.TraderId && order.BuyerId != request.TraderId)
            throw new InvariantViolationException("Trader is neither a buyer nor a seller.");
        
        return new GetSellOrderResponse { SellOrder = order };
    }
    
    private readonly IUnitOfWork _unitOfWork;
}