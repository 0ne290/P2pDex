using Core.Application.SellOrder.Commands;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.SellOrder.Handlers;

public class RespondToSellOrderByBuyerHandler : IRequestHandler<RespondToSellOrderByBuyerCommand, CommandResult>
{
    public RespondToSellOrderByBuyerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<CommandResult> Handle(RespondToSellOrderByBuyerCommand request, CancellationToken _)
    {
        var order = await _unitOfWork.Repository.TryGet<Domain.Entities.SellOrder>(o => o.Guid.Equals(request.OrderGuid));

        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        
        order.RespondByBuyer(request.BuyerId, request.BuyerAccountAddress);
        
        if (!await _unitOfWork.Repository.Exists<Trader>(t => t.Id.Equals(request.BuyerId)))
            throw new InvariantViolationException("Buyer does not exists.");
    
        await _unitOfWork.SaveAllTrackedEntities();

        return new CommandResult(new { guid = order.Guid, status = order.Status.ToString() });
    }

    private readonly IUnitOfWork _unitOfWork;
}
