using Core.Application.Commands;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.Handlers;

public class CalculateFinalCryptoAmountForTransferHandler : IRequestHandler<CalculateFinalCryptoAmountForTransferCommand, CommandResult>
{
    public CalculateFinalCryptoAmountForTransferHandler(IBlockchain blockchain, decimal feeRate)
    {
        _blockchain = blockchain;
        _feeRate = feeRate;
    }

    public Task<CommandResult> Handle(CalculateFinalCryptoAmountForTransferCommand request, CancellationToken _)
    {
        var exchangerToMinersFee = _blockchain.TransferTransactionFee;
        var sellerToExchangerFee = request.CryptoAmount * _feeRate;
        var finalCryptoAmount = request.CryptoAmount + exchangerToMinersFee.Value + sellerToExchangerFee;

        return Task.FromResult(new CommandResult(new
            { finalCryptoAmount, relevanceTimeInMs = exchangerToMinersFee.TimeToUpdateInMs }));
    }
    
    private readonly IBlockchain _blockchain;

    private readonly decimal _feeRate;
}