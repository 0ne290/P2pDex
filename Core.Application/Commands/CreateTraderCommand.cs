using FluentResults;
using MediatR;

namespace Core.Application.Commands;

public class CreateTraderCommand : IRequest<Result<Guid>>
{
    public required string Name { get; init; }
}