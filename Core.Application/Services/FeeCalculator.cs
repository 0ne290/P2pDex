using Core.Application.Interfaces;

namespace Core.Application.Services;

public class FeeCalculator : IFeeCalculator
{
    public FeeCalculator(IBlockchain blockchain, decimal feeRate)
    {
        _blockchain = blockchain;
        _feeRate = feeRate;
    }
    
    public async Task<decimal> Calculate(decimal cryptoAmount)
    {
        var transferTransactionFee = await _blockchain.GetTransferTransactionFee();

        return transferTransactionFee + cryptoAmount * _feeRate;
    }

    private readonly IBlockchain _blockchain;

    private readonly decimal _feeRate;
}