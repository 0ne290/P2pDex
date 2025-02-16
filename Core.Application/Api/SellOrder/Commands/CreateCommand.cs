using MediatR;
using Newtonsoft.Json;

namespace Core.Application.UseCases.SellOrder.Commands;

public class CreateSellOrderCommand : IRequest<IDictionary<string, object>>
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
    
    [JsonProperty(Required = Required.Always, PropertyName = "sellerId")]
    public required long SellerId { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "transferTransactionHash")]
    public required string TransferTransactionHash { get; init; }
}