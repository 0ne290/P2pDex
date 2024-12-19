using Core.Domain.Interfaces;
using Core.Domain.Models;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;

namespace Infrastructure.Blockchain;

public class EthereumBlockchain : IBlockchain
{
    static EthereumBlockchain()
    {
        var priorityFeeValueInWei = Web3.Convert.ToWei(2, UnitConversion.EthUnit.Gwei);
        PriorityFee = (priorityFeeValueInWei.ToHexBigInteger(), Web3.Convert.FromWei(priorityFeeValueInWei));
    }
    
    public EthereumBlockchain(Web3 web3, FeePerGasTracker feePerGasTracker)
    {
        _web3 = web3;
        _feePerGasTracker = feePerGasTracker;
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
        var transactionInput = new TransactionInput(new HexBigInteger(2), null, to, from,
            new HexBigInteger(GasLimitOfTransferTransaction), Web3.Convert.ToWei(amount).ToHexBigInteger(),
            _feePerGasTracker.Value.InWei, PriorityFee.ValueInWei);
        
        //await _web3.TransactionManager.SendTransactionAsync(AccountAddress, to,
        //    Web3.Convert.ToWei(amount).ToHexBigInteger());

        return await _web3.TransactionManager.SendTransactionAsync(transactionInput);
    }

    public (decimal Value, double TimeToUpdateInMs) TransferTransactionFee => (
        GasLimitOfTransferTransaction * (PriorityFee.ValueInEth + _feePerGasTracker.Value.InEth),
        _feePerGasTracker.TimeToUpdateInMs);

    private static readonly (HexBigInteger ValueInWei, decimal ValueInEth) PriorityFee;
    
    public const int GasLimitOfTransferTransaction = 21_000;
    
    private readonly Web3 _web3;

    private readonly FeePerGasTracker _feePerGasTracker;
}