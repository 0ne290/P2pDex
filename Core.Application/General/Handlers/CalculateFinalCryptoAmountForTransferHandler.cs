using System.Dynamic;
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
        
        dynamic ret = new ExpandoObject();
        ret.finalCryptoAmountguid = finalCryptoAmount;
        ret.relevanceTimeInMs = exchangerToMinersFee.TimeToUpdateInMs;

        return Task.FromResult(new CommandResult(ret));
    }
    
    private readonly IBlockchain _blockchain;

    private readonly ExchangerConfiguration _exchangerConfiguration;
}