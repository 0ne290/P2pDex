/*using MediatR;
using Newtonsoft.Json;

namespace Core.Application.BuyOrder.Commands;

public class RespondToBuyOrderBySellerCommand : IRequest<CommandResult>
{
    [JsonProperty(Required = Required.Always, PropertyName = "orderGuid")]
    public required Guid OrderGuid { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "sellerGuid")]
    public required Guid SellerGuid { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "transferTransactionHash")]
    public required string TransferTransactionHash { get; init; }
}*/