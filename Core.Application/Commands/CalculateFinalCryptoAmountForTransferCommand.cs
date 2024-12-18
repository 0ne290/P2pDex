using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Commands;

public class CalculateFinalCryptoAmountForTransferCommand : IRequest<CommandResult>
{
    [JsonProperty(Required = Required.Always, PropertyName = "cryptoAmount")]
    public required decimal CryptoAmount { get; init; }
}