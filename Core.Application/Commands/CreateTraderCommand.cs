using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Commands;

public class CreateTraderCommand : IRequest<CommandResult>
{
    [JsonProperty(PropertyName = "name")]
    public required string Name { get; init; }
}