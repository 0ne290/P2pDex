using Core.Domain.Interfaces;
using Core.Domain.Models;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;

namespace Infrastructure.Blockchain;

public class EthereumBlockchain : IBlockchain
{
    public EthereumBlockchain(Web3 web3, FeeTracker feeTracker)
    {
        _web3 = web3;
        _feeTracker = feeTracker;
    }

    public async Task<TransferTransaction?> TryGetConfirmedTransactionByHash(string transactionHash)
    {
        var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

        if (receipt == null || receipt.Status.Value != 1)
            return null;
        
        var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);

        return new TransferTransaction
        {
            From = transaction.From,
            To = transaction.To,
            Amount = Web3.Convert.FromWei(transaction.Value)
        };
    }

    public async Task<string> SendTransferTransaction(string from, string to, decimal amount)
    {
        var transactionInput = new TransactionInput(Eip1559TransactionType, null, to, from,
            GasLimitOfTransferTransaction, Web3.Convert.ToWei(amount).ToHexBigInteger(), _feeTracker.MaxFeePerGasInWei,
            MaxPriorityFeePerGasInWei);
        
        return await _web3.TransactionManager.SendTransactionAsync(transactionInput);
    }

    public (decimal Value, double TimeToUpdateInMs) TransferTransactionFee =>
        (_feeTracker.TransferTransactionFeeInEth, _feeTracker.TimeToUpdateInMs);

    private static readonly HexBigInteger Eip1559TransactionType = new(2);

    public static readonly HexBigInteger MaxPriorityFeePerGasInWei =
        Web3.Convert.ToWei(2, UnitConversion.EthUnit.Gwei).ToHexBigInteger();
    
    public static readonly HexBigInteger GasLimitOfTransferTransaction = new(21_000);
    
    private readonly Web3 _web3;

    private readonly FeeTracker _feeTracker;
}