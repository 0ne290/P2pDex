using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Api.SellOrder.Commands;

public class GetAllSellOrdersCommand : IRequest<IDictionary<string, object>>
{
    [JsonProperty(Required = Required.Always, PropertyName = "traderId")]
    public required long TraderId { get; init; }
}