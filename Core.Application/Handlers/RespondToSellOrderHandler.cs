using Core.Application.Commands;
using MediatR;

namespace Core.Application.Handlers;

public class RespondToSellOrderHandler : IRequestHandler<RespondToSellOrderCommand, CommandResult>
{
    public Task<CommandResult> Handle(RespondToSellOrderCommand request, CancellationToken _)
    {
        throw new NotImplementedException();
    }
}