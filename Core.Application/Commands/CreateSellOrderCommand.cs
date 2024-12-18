using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Commands;

public class CreateSellOrderCommand : IRequest<CommandResult>
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
    
    [JsonProperty(Required = Required.Always, PropertyName = "sellerGuid")]
    public required Guid SellerGuid { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "transferTransactionHash")]
    public required string TransferTransactionHash { get; init; }
}