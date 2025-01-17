using MediatR;
using Newtonsoft.Json;

namespace Core.Application.General.Commands;

public class CreateTraderCommand : IRequest<CommandResult>
{
    [JsonProperty(Required = Required.Always, PropertyName = "name")]
    public required string Name { get; init; }
}