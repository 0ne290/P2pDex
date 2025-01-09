using Core.Application.General.Commands;
using MediatR;

namespace Core.Application.General.Handlers;

public class GetExchangerAccountAddressHandler : IRequestHandler<GetExchangerAccountAddressCommand, CommandResult>
{
    public GetExchangerAccountAddressHandler(ExchangerConfiguration exchangerConfiguration)
    {
        _exchangerConfiguration = exchangerConfiguration;
    }

    public Task<CommandResult> Handle(GetExchangerAccountAddressCommand _, CancellationToken __) =>
        Task.FromResult(new CommandResult(new { accountAddress = _exchangerConfiguration.AccountAddress }));

    private readonly ExchangerConfiguration _exchangerConfiguration;
}