using MediatR;

namespace Core.Application.Commands;

public class CreateTraderCommand : IRequest<CommandResult>
{
    public required string Name { get; init; }
}