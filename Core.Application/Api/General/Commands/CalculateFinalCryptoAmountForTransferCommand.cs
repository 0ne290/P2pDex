using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Api.General.Commands;

public class CalculateFinalCryptoAmountForTransferCommand : IRequest<CalculateFinalCryptoAmountForTransferResponse>
{
    [JsonProperty(Required = Required.Always, PropertyName = "cryptoAmount")]
    public required decimal CryptoAmount { get; init; }
}

public class CalculateFinalCryptoAmountForTransferResponse
{
    [JsonProperty(Required = Required.Always, PropertyName = "finalCryptoAmount")]
    public required decimal FinalCryptoAmount { get; init; }
    
    [JsonProperty(Required = Required.Always, PropertyName = "relevanceTimeInMs")]
    public required int RelevanceTimeInMs { get; init; }
}