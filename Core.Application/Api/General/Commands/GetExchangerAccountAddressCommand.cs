using MediatR;
using Newtonsoft.Json;

namespace Core.Application.Api.General.Commands;

public class GetExchangerAccountAddressCommand : IRequest<GetExchangerAccountAddressResponse>;

public class GetExchangerAccountAddressResponse
{
    [JsonProperty(Required = Required.AllowNull, PropertyName = "accountAddress")]
    public required string AccountAddress { get; init; }
}