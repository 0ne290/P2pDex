using Core.Domain.Interfaces;
using Core.Domain.Models;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.TransactionTypes;
using Nethereum.Web3;

namespace Infrastructure.Blockchain;

public class EthereumBlockchain : IBlockchain
{
    public EthereumBlockchain(Web3 web3, string accountAddress, FeeTracker feeTracker)
    {
        _web3 = web3;
        _accountAddress = accountAddress;
        _feeTracker = feeTracker;
        _currentNonce = web3.Eth.Transactions.GetTransactionCount
            .SendRequestAsync(_accountAddress, BlockParameter.CreatePending()).GetAwaiter()
            .GetResult();
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

    public async Task<string> SendTransferTransaction(string to, decimal amount)
    {
        try
        {
            TransactionInput transactionInput;
            lock (_locker)
            {
                transactionInput = new TransactionInput(TransactionType.EIP1559.AsHexBigInteger(), null, to, _accountAddress,
                    _feeTracker.GasLimitOfTransferTransaction, Web3.Convert.ToWei(amount).ToHexBigInteger(),
                    _feeTracker.MaxFeePerGasInWei, _feeTracker.MaxPriorityFeePerGasInWei) { Nonce = _currentNonce };

                _currentNonce = (_currentNonce.Value + 1).ToHexBigInteger();
            }

            var transactionHash = await _web3.TransactionManager.SendTransactionAsync(transactionInput);

            return transactionHash;
        }
        catch (Exception)
        {
            var currentNonce =
                await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(
                    _accountAddress, BlockParameter.CreatePending());
            lock (_locker)
            {
                _currentNonce = currentNonce;
            }
            
            throw;
        }
    }

    public (decimal Value, double TimeToUpdateInMs) TransferTransactionFee =>
        (_feeTracker.TransferTransactionFeeInEth, _feeTracker.TimeToUpdateInMs);
    
    private readonly Web3 _web3;

    private readonly string _accountAddress;

    private readonly FeeTracker _feeTracker;

    private HexBigInteger _currentNonce;

    private readonly object _locker = new();
}