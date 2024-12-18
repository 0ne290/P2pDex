using System.Numerics;
using Core.Domain.Interfaces;
using Core.Domain.Models;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;

namespace Infrastructure.Blockchain;

public class EthereumBlockchain : IBlockchain
{
    public EthereumBlockchain(Web3 web3, TransferTransactionFeeTracker transferTransactionFeeTracker)
    {
        _web3 = web3;
        _transferTransactionFeeTracker = transferTransactionFeeTracker;
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
        var transactionInput = new TransactionInput(null, to, from,
            new HexBigInteger(new BigInteger(TransferTransactionFeeTracker.GasLimitOfTransferTransaction)),
            new HexBigInteger(Web3.Convert
                .ToWei(TransferTransactionFee.Value / TransferTransactionFeeTracker.GasLimitOfTransferTransaction)
                .ToHexBigInteger()), Web3.Convert.ToWei(amount).ToHexBigInteger());
        
        //await _web3.TransactionManager.SendTransactionAsync(AccountAddress, to,
        //    Web3.Convert.ToWei(amount).ToHexBigInteger());

        return await _web3.TransactionManager.SendTransactionAsync(transactionInput);
    }

    public (decimal Value, double TimeToUpdateInMs) TransferTransactionFee => (_transferTransactionFeeTracker.Fee,
        _transferTransactionFeeTracker.TimeToUpdateInMs);

    private static readonly HexBigInteger PriorityFee =
        new(Web3.Convert.ToWei(2, UnitConversion.EthUnit.Gwei));
    
    public const decimal GasLimitOfTransferTransaction = 21_000m;
    
    private readonly Web3 _web3;

    private readonly TransferTransactionFeeTracker _transferTransactionFeeTracker;
}