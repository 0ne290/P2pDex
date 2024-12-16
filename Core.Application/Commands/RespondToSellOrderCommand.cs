using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Commands;

public class RespondToSellOrderCommand : IRequest<CommandResult>
{
    [JsonProperty(PropertyName = "orderGuid")]
    public required Guid OrderGuid { get; init; }
    
    [JsonProperty(PropertyName = "buyerGuid")]
    public required Guid BuyerGuid { get; init; }
    
    [JsonProperty(PropertyName = "buyerAccountAddress")]
    public required string BuyerAccountAddress { get; init; }
}