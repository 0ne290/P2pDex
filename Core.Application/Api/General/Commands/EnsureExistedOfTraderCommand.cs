using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Api.General.Commands;

public class EnsureExistedOfTraderCommand : IRequest<EnsureExistedOfTraderResponse>
{
    [JsonProperty(Required = Required.Always, PropertyName = "id")]
    public required long Id { get; init; }
    
    [JsonProperty(Required = Required.AllowNull, PropertyName = "name")]
    public required string? Name { get; init; }
}

public class EnsureExistedOfTraderResponse
{
    [JsonProperty(Required = Required.AllowNull, PropertyName = "message")]
    public required string Message { get; init; }
}