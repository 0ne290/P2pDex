using Core.Domain.Constants;
using Newtonsoft.Json;

namespace Core.Application.Api.SellOrder.Get;

public class SellOrderAndItsTradersDto
{
    [JsonProperty(Required = Required.Always, PropertyName = "status")]
    public required OrderStatus Status { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "sellerId")]
    public required long SellerId { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "sellerName")]
    public required string? SellerName { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "buyerId")]
    public required long? BuyerId { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "buyerName")]
    public required string? BuyerName { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "crypto")]
    public required Cryptocurrency Crypto { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "cryptoAmount")]
    public required decimal CryptoAmount { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "fiat")]
    public required FiatCurrency Fiat { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "cryptoToFiatExchangeRate")]
    public required decimal CryptoToFiatExchangeRate { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "fiatAmount")]
    public required decimal FiatAmount { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "paymentMethodInfo")]
    public required string PaymentMethodInfo { get; init; }
}