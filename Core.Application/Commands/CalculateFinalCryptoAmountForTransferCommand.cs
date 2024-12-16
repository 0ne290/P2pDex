using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Commands;

public class CalculateFinalCryptoAmountForTransferCommand : IRequest<CommandResult>
{
    [JsonProperty(PropertyName = "cryptoAmount")]
    public required decimal CryptoAmount { get; init; }
}