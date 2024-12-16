using Core.Application.Commands;
using MediatR;

namespace Core.Application.Handlers;

public class RespondToSellOrderHandler : IRequestHandler<RespondToSellOrderCommand, CommandResult>
{
    public Task<CommandResult> Handle(RespondToSellOrderCommand request, CancellationToken _)
    {
        var ethereumAccountAddressRegex = new Regex("^0x[0-9a-fA-F]{40}$");
    
        var order = await _unitOfWork.Repository.TryGetByGuid<SellOrder>(request.OrderGuid);
        order.Respond(request.BuyerGuid, request.BuyerAccountAddress);

        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        if (!await _unitOfWork.Repository.Exists<Trader>(t => t.Guid == request.BuyerGuid))
            throw new InvariantViolationException("Buyer does not exists.");
        // TODO: вынести эту проверку в сущность.
        if (!ethereumAccountAddressRegex.Match(request.BuyerAccountAddress))
            throw new InvariantViolationException("Buyer account address is invalid.");
    
        await _unitOfWork.Save();

        return new CommansResult(new { guid = order.Guid, status = order.Status });
    }
}
