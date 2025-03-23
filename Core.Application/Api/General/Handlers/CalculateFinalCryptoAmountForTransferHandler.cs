using Core.Application.Api.General.Commands;
using Core.Application.Private.Configurations;
using Core.Application.Private.Interfaces;
using MediatR;

namespace Core.Application.Api.General.Handlers;

public class CalculateFinalCryptoAmountForTransferHandler : IRequestHandler<CalculateFinalCryptoAmountForTransferCommand, CalculateFinalCryptoAmountForTransferResponse>
{
    public CalculateFinalCryptoAmountForTransferHandler(IBlockchain blockchain, ExchangerConfiguration exchangerConfiguration)
    {
        _blockchain = blockchain;
        _exchangerConfiguration = exchangerConfiguration;
    }

    public Task<CalculateFinalCryptoAmountForTransferResponse> Handle(CalculateFinalCryptoAmountForTransferCommand request, CancellationToken _)
    {
        var exchangerToMinersFee = _blockchain.GetTransferTransactionFee(DateTime.Now);
        var sellerToExchangerFee = request.CryptoAmount * _exchangerConfiguration.FeeRate;
        var finalCryptoAmount = request.CryptoAmount + exchangerToMinersFee.Value + sellerToExchangerFee;

        return Task.FromResult(new CalculateFinalCryptoAmountForTransferResponse
        {
            FinalCryptoAmount = finalCryptoAmount, RelevanceTimeInMs = exchangerToMinersFee.TimeToUpdateInMs
        });
    }
    
    private readonly IBlockchain _blockchain;

    private readonly ExchangerConfiguration _exchangerConfiguration;
}