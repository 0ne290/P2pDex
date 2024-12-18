using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Commands;

public class ConfirmOrderByBuyerCommand : IRequest<CommandResult>
{
    [JsonProperty(Required = Required.Always, PropertyName = "orderGuid")]
    public required Guid OrderGuid { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "buyerGuid")]
    public required Guid BuyerGuid { get; init; }
}