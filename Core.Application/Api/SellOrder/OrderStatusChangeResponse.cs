using Core.Domain.Constants;
using Newtonsoft.Json;

namespace Core.Application.Api.SellOrder;

public class OrderStatusChangeResponse
{
    [JsonProperty(Required = Required.Always, PropertyName = "guid")]
    public required Guid Guid { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "status")]
    public required OrderStatus Status { get; init; }
}