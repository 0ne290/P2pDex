using Core.Domain.Constants;
using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Api.SellOrder.Commands;

public class GetAllSellOrdersCommand : IRequest<GetAllSellOrdersResponse>
{
    [JsonProperty(Required = Required.Always, PropertyName = "traderId")]
    public required long TraderId { get; init; }
}

public class GetAllSellOrdersResponse
{
    [JsonProperty(Required = Required.Always, PropertyName = "sellOrders")]
    public required ICollection<SellOrderWithSeller> SellOrders { get; init; }
}

public class SellOrderWithSeller
{
    [JsonProperty(Required = Required.Always, PropertyName = "sellerId")]
    public required long SellerId { get; init; }

    [JsonProperty(Required = Required.AllowNull, PropertyName = "sellerName")]
    public required string? SellerName { get; init; }

    [JsonProperty(Required = Required.Always, PropertyName = "guid")]
    public required Guid Guid { get; init; }

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