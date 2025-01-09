using MediatR;
using Newtonsoft.Json;

namespace Core.Application.SellOrder.Commands;

public class ConfirmReceiptFiatFromBuyerBySellerForSellOrderCommand : IRequest<CommandResult>
{
    [JsonProperty(Required = Required.Always, PropertyName = "orderGuid")]
    public required Guid OrderGuid { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "sellerGuid")]
    public required Guid SellerGuid { get; init; }
}
