using System.Dynamic;
using Core.Application.General.Commands;
using MediatR;

namespace Core.Application.General.Handlers;

public class GetExchangerAccountAddressHandler : IRequestHandler<GetExchangerAccountAddressCommand, CommandResult>
{
    public GetExchangerAccountAddressHandler(ExchangerConfiguration exchangerConfiguration)
    {
        _exchangerConfiguration = exchangerConfiguration;
    }

    public Task<CommandResult> Handle(GetExchangerAccountAddressCommand _, CancellationToken __)
    {
        dynamic ret = new ExpandoObject();
        ret.accountAddress = _exchangerConfiguration.AccountAddress;
        
        return Task.FromResult(new CommandResult(ret));
    }

    private readonly ExchangerConfiguration _exchangerConfiguration;
}