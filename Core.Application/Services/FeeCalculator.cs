using Core.Domain.Interfaces;

namespace Core.Application.Services;

public class FeeCalculator
{
    public FeeCalculator(IBlockchain blockchain, decimal feeRate)
    {
        _blockchain = blockchain;
        _feeRate = feeRate;
    }
    
    public async Task<(decimal SellerToExchanger, decimal ExpectedExchangerToMiners)> Calculate(decimal cryptoAmount)
    {
        var transferTransactionFee = await _blockchain.GetTransferTransactionFee();

        return (cryptoAmount * _feeRate, transferTransactionFee);
    }

    private readonly IBlockchain _blockchain;

    private readonly decimal _feeRate;
}