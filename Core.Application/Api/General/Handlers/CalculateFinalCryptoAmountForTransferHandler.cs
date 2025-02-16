using Core.Application.Configurations;
using Core.Application.Interfaces;
using Core.Application.UseCases.General.Commands;
using MediatR;

namespace Core.Application.UseCases.General.Handlers;

public class CalculateFinalCryptoAmountForTransferHandler : IRequestHandler<CalculateFinalCryptoAmountForTransferCommand, IDictionary<string, object>>
{
    public CalculateFinalCryptoAmountForTransferHandler(IBlockchain blockchain, ExchangerConfiguration exchangerConfiguration)
    {
        _blockchain = blockchain;
        _exchangerConfiguration = exchangerConfiguration;
    }

    public Task<IDictionary<string, object>> Handle(CalculateFinalCryptoAmountForTransferCommand request, CancellationToken _)
    {
        var exchangerToMinersFee = _blockchain.TransferTransactionFee;
        var sellerToExchangerFee = request.CryptoAmount * _exchangerConfiguration.FeeRate;
        var finalCryptoAmount = request.CryptoAmount + exchangerToMinersFee.Value + sellerToExchangerFee;
        
        IDictionary<string, object> ret = new Dictionary<string, object>
        {
            ["finalCryptoAmount"] = finalCryptoAmount,
            ["relevanceTimeInMs"] = exchangerToMinersFee.TimeToUpdateInMs
        };

        return Task.FromResult<IDictionary<string, object>>(ret);
    }
    
    private readonly IBlockchain _blockchain;

    private readonly ExchangerConfiguration _exchangerConfiguration;
}