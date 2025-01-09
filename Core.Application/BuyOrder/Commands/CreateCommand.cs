using MediatR;
using Newtonsoft.Json;

namespace Core.Application.BuyOrder.Commands;

public class CreateBuyOrderCommand : IRequest<CommandResult>
{
    [JsonProperty(Required = Required.Always, PropertyName = "crypto")]
    public required string Crypto { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "cryptoAmount")]
    public required decimal CryptoAmount { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "fiat")]
    public required string Fiat { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "cryptoToFiatExchangeRate")]
    public required decimal CryptoToFiatExchangeRate { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "paymentMethodInfo")]
    public required string PaymentMethodInfo { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "buyerGuid")]
    public required Guid BuyerGuid { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "buyerAccountAddress")]
    public required string BuyerAccountAddress { get; init; }
}