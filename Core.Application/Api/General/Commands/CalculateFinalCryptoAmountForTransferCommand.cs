using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Api.General.Commands;

public class CalculateFinalCryptoAmountForTransferCommand : IRequest<IDictionary<string, object>>
{
    [JsonProperty(Required = Required.Always, PropertyName = "cryptoAmount")]
    public required decimal CryptoAmount { get; init; }
}