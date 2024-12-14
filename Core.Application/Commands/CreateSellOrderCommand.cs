using Core.Domain.Enums;
using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Commands;

public class CreateSellOrderCommand : IRequest<CommandResult>
{
    [JsonProperty(PropertyName = "crypto")]
    public required Cryptocurrency Crypto { get; init; }

    [JsonProperty(PropertyName = "cryptoAmount")]
    public required decimal CryptoAmount { get; init; }

    [JsonProperty(PropertyName = "fiat")]
    public required FiatCurrency Fiat { get; init; }

    [JsonProperty(PropertyName = "cryptoToFiatExchangeRate")]
    public required decimal CryptoToFiatExchangeRate { get; init; }

    [JsonProperty(PropertyName = "paymentMethodInfo")]
    public required string PaymentMethodInfo { get; init; }
    
    [JsonProperty(PropertyName = "sellerGuid")]
    public required Guid SellerGuid { get; init; }
    
    [JsonProperty(PropertyName = "transferTransactionHash")]
    public required string TransferTransactionHash { get; init; }
}