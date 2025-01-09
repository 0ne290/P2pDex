using MediatR;
using Newtonsoft.Json;

namespace Core.Application.SellOrder.Commands;

public class RespondToSellOrderByBuyerCommand : IRequest<CommandResult>
{
    [JsonProperty(Required = Required.Always, PropertyName = "orderGuid")]
    public required Guid OrderGuid { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "buyerGuid")]
    public required Guid BuyerGuid { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "buyerAccountAddress")]
    public required string BuyerAccountAddress { get; init; }
}