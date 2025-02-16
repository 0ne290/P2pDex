using Core.Application.Configurations;
using Core.Application.UseCases.General.Commands;
using MediatR;

namespace Core.Application.UseCases.General.Handlers;

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