using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Commands;

public class ConfirmBySellerAndCompleteOrderCommand : IRequest<CommandResult>
{
    [JsonProperty(PropertyName = "orderGuid")]
    public required Guid OrderGuid { get; init; }
    
    [JsonProperty(PropertyName = "sellerGuid")]
    public required Guid SellerGuid { get; init; }
}
