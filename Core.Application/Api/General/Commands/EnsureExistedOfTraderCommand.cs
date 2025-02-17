using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Api.General.Commands;

public class EnsureExistedOfTraderCommand : IRequest<IDictionary<string, object>>
{
    [JsonProperty(Required = Required.Always, PropertyName = "id")]
    public required long Id { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "name")]
    public required string? Name { get; init; }
}