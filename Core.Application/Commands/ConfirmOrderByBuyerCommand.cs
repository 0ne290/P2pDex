using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Commands;

public class ConfirmOrderByBuyerCommand : IRequest<CommandResult>
{
    [JsonProperty(PropertyName = "orderGuid")]
    public required Guid OrderGuid { get; init; }
    
    [JsonProperty(PropertyName = "buyerGuid")]
    public required Guid BuyerGuid { get; init; }
}