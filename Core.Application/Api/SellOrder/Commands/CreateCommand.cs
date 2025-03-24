using Core.Domain.Constants;
using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Api.SellOrder.Commands;

public class CreateSellOrderCommand : IRequest<OrderStatusChangeResponse>
{
    [JsonProperty(Required = Required.Always, PropertyName = "crypto")]
    public required Cryptocurrency Crypto { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "cryptoAmount")]
    public required decimal CryptoAmount { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "fiat")]
    public required FiatCurrency Fiat { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "cryptoToFiatExchangeRate")]
    public required decimal CryptoToFiatExchangeRate { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "paymentMethodInfo")]
    public required string PaymentMethodInfo { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "sellerId")]
    public required long SellerId { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "transferTransactionHash")]
    public required string TransferTransactionHash { get; init; }
}