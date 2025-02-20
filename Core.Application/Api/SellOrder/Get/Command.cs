using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Api.SellOrder.Get;

public class GetSellOrderCommand : IRequest<IDictionary<string, object>>
{
    [JsonProperty(Required = Required.Always, PropertyName = "orderGuid")]
    public required Guid OrderGuid { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "traderId")]
    public required long TraderId { get; init; }
}