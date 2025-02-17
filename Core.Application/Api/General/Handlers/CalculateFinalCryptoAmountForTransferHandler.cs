using Core.Application.Api.General.Commands;
using Core.Application.Private.Configurations;
using Core.Application.Private.Interfaces;
using MediatR;

namespace Core.Application.Api.General.Handlers;

public class CalculateFinalCryptoAmountForTransferHandler : IRequestHandler<CalculateFinalCryptoAmountForTransferCommand, IDictionary<string, object>>
{
    public CalculateFinalCryptoAmountForTransferHandler(IBlockchain blockchain, ExchangerConfiguration exchangerConfiguration)
    {
        _blockchain = blockchain;
        _exchangerConfiguration = exchangerConfiguration;
    }

    public Task<IDictionary<string, object>> Handle(CalculateFinalCryptoAmountForTransferCommand request, CancellationToken _)
    {
        var exchangerToMinersFee = _blockchain.GetTransferTransactionFee(DateTime.Now);
        var sellerToExchangerFee = request.CryptoAmount * _exchangerConfiguration.FeeRate;
        var finalCryptoAmount = request.CryptoAmount + exchangerToMinersFee.Value + sellerToExchangerFee;
        
        IDictionary<string, object> ret = new Dictionary<string, object>
        {
            ["finalCryptoAmount"] = finalCryptoAmount,
            ["relevanceTimeInMs"] = exchangerToMinersFee.TimeToUpdateInMs
        };

        return Task.FromResult(ret);
    }
    
    private readonly IBlockchain _blockchain;

    private readonly ExchangerConfiguration _exchangerConfiguration;
}