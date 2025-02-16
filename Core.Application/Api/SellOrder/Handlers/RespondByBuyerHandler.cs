using Core.Application.Interfaces;
using Core.Application.UseCases.SellOrder.Commands;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;

namespace Core.Application.UseCases.SellOrder.Handlers;

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

        var ret = new Dictionary<string, object>
        {
            ["guid"] = order.Guid,
            ["status"] = order.Status.ToString()
        };
        
        return new CommandResult(ret);
    }

    private readonly IUnitOfWork _unitOfWork;
}
