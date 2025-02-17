using Core.Application.Api.General.Commands;
using Core.Application.Private.Configurations;
using MediatR;

namespace Core.Application.Api.General.Handlers;

public class GetExchangerAccountAddressHandler : IRequestHandler<GetExchangerAccountAddressCommand, IDictionary<string, object>>
{
    public GetExchangerAccountAddressHandler(ExchangerConfiguration exchangerConfiguration)
    {
        _exchangerConfiguration = exchangerConfiguration;
    }

    public Task<IDictionary<string, object>> Handle(GetExchangerAccountAddressCommand _, CancellationToken __)
    {
        IDictionary<string, object> ret = new Dictionary<string, object>
        {
            ["accountAddress"] = _exchangerConfiguration.AccountAddress
        };

        return Task.FromResult(ret);
    }

    private readonly ExchangerConfiguration _exchangerConfiguration;
}