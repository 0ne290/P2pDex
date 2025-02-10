using MediatR;
using Newtonsoft.Json;

namespace Core.Application.General.Commands;

public class EnsureExistedOfTraderCommand : IRequest<CommandResult>
{
    [JsonProperty(Required = Required.Always, PropertyName = "id")]
    public required long Id { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "name")]
    public required string? Name { get; init; }
}