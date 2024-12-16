using Core.Application.Commands;
using Core.Domain.Services;
using MediatR;

namespace Core.Application.Handlers;

public class CalculateFinalCryptoAmountForTransferHandler : IRequestHandler<CalculateFinalCryptoAmountForTransferCommand, CommandResult>
{
    public CalculateFinalCryptoAmountForTransferHandler(Exchanger exchanger)
    {
        _exchanger = exchanger;
    }

    public Task<CommandResult> Handle(CalculateFinalCryptoAmountForTransferCommand request, CancellationToken _)
    {
        var ret = _exchanger.CalculateFinalCryptoAmountForTransfer(request.CryptoAmount);

        return Task.FromResult(new CommandResult(new
            { finalCryptoAmount = ret.FinalCryptoAmount, relevanceTimeInMs = ret.RelevanceTimeInMs }));
    }
        

    private readonly Exchanger _exchanger;
}