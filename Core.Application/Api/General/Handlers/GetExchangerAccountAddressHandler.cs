using Core.Application.Api.General.Commands;
using Core.Application.Private.Configurations;
using MediatR;

namespace Core.Application.Api.General.Handlers;

public class GetExchangerAccountAddressHandler : IRequestHandler<GetExchangerAccountAddressCommand, GetExchangerAccountAddressResponse>
{
    public GetExchangerAccountAddressHandler(ExchangerConfiguration exchangerConfiguration)
    {
        _exchangerConfiguration = exchangerConfiguration;
    }

    public Task<GetExchangerAccountAddressResponse> Handle(GetExchangerAccountAddressCommand _, CancellationToken __)
    {
        return Task.FromResult(new GetExchangerAccountAddressResponse { AccountAddress = _exchangerConfiguration.AccountAddress });
    }

    private readonly ExchangerConfiguration _exchangerConfiguration;
}