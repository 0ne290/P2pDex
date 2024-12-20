using Core.Application.Commands;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.Handlers;

public class RespondToSellOrderHandler : IRequestHandler<RespondToSellOrderCommand, CommandResult>
{
    public RespondToSellOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<CommandResult> Handle(RespondToSellOrderCommand request, CancellationToken _)
    {
        var order = await _unitOfWork.Repository.TryGetByGuid<SellOrder>(request.OrderGuid);

        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        
        order.Respond(request.BuyerGuid, request.BuyerAccountAddress);
        
        if (!await _unitOfWork.Repository.Exists<Trader>(t => t.Guid.Equals(request.BuyerGuid)))
            throw new InvariantViolationException("Buyer does not exists.");
    
        await _unitOfWork.Save();

        return new CommandResult(new { guid = order.Guid, status = order.Status.ToString() });
    }

    private readonly IUnitOfWork _unitOfWork;
}
