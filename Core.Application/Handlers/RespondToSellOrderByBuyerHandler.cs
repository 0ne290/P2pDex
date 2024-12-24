using Core.Application.Commands;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.Handlers;

public class RespondToSellOrderByBuyerHandler : IRequestHandler<RespondToSellOrderByBuyerCommand, CommandResult>
{
    public RespondToSellOrderByBuyerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<CommandResult> Handle(RespondToSellOrderByBuyerCommand request, CancellationToken _)
    {
        var order = await _unitOfWork.Repository.TryGetByGuid<SellOrder>(request.OrderGuid);

        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        
        order.RespondByBuyer(request.BuyerGuid, request.BuyerAccountAddress);
        
        if (!await _unitOfWork.Repository.Exists<Trader>(t => t.Guid.Equals(request.BuyerGuid)))
            throw new InvariantViolationException("Buyer does not exists.");
    
        await _unitOfWork.SaveAllTrackedEntities();

        return new CommandResult(new { guid = order.Guid, status = order.Status.ToString() });
    }

    private readonly IUnitOfWork _unitOfWork;
}
