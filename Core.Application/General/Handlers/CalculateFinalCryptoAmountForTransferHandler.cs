using Core.Application.General.Commands;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.General.Handlers;

public class CalculateFinalCryptoAmountForTransferHandler : IRequestHandler<CalculateFinalCryptoAmountForTransferCommand, CommandResult>
{
    public CalculateFinalCryptoAmountForTransferHandler(IBlockchain blockchain, ExchangerConfiguration exchangerConfiguration)
    {
        _blockchain = blockchain;
        _exchangerConfiguration = exchangerConfiguration;
    }

    public Task<CommandResult> Handle(CalculateFinalCryptoAmountForTransferCommand request, CancellationToken _)
    {
        var exchangerToMinersFee = _blockchain.TransferTransactionFee;
        var sellerToExchangerFee = request.CryptoAmount * _exchangerConfiguration.FeeRate;
        var finalCryptoAmount = request.CryptoAmount + exchangerToMinersFee.Value + sellerToExchangerFee;

        return Task.FromResult(new CommandResult(new
            { finalCryptoAmount, relevanceTimeInMs = exchangerToMinersFee.TimeToUpdateInMs }));
    }
    
    private readonly IBlockchain _blockchain;

    private readonly ExchangerConfiguration _exchangerConfiguration;
}