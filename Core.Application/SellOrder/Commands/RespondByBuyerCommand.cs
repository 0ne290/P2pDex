using MediatR;
using Newtonsoft.Json;

namespace Core.Application.SellOrder.Commands;

public class RespondToSellOrderByBuyerCommand : IRequest<CommandResult>
{
    [JsonProperty(Required = Required.Always, PropertyName = "orderGuid")]
    public required Guid OrderGuid { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "buyerId")]
    public required long BuyerId { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "buyerAccountAddress")]
    public required string BuyerAccountAddress { get; init; }
}