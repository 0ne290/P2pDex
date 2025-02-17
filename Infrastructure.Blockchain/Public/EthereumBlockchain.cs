using Core.Application.Private.Constants;
using Core.Application.Private.Interfaces;
using Core.Application.Private.Models;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.TransactionTypes;
using Nethereum.Util;
using Nethereum.Web3;

namespace Infrastructure.Blockchain.Public;

public class EthereumBlockchain : IBlockchain
{
    public EthereumBlockchain(Web3 web3, string accountAddress)
    {
        _web3 = web3;
        _accountAddress = accountAddress;
        _currentNonce = web3.Eth.Transactions.GetTransactionCount
            .SendRequestAsync(_accountAddress, BlockParameter.CreatePending()).GetAwaiter()
            .GetResult();
    }

    public async Task<TransferTransaction?> TryGetTransactionByHash(string transactionHash)
    {
        var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);

        if (transaction == null)
            return null;

        var transactionReceipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
        TransferTransactionStatus transactionStatus;
        if (transactionReceipt == null)
            transactionStatus = TransferTransactionStatus.InProcess;
        else
            transactionStatus = transactionReceipt.Status.Value == 1
                ? TransferTransactionStatus.Confirmed
                : TransferTransactionStatus.Rejected;

        return new TransferTransaction
        {
            Status = transactionStatus,
            From = transaction.From,
            To = transaction.To,
            Amount = Web3.Convert.FromWei(transaction.Value)
        };
    }

    public async Task<TransferTransactionStatus?> TryGetTransactionStatus(string transactionHash)
    {
        var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

        if (receipt == null)
            return null;

        return receipt.Status.Value == 1 ? TransferTransactionStatus.Confirmed : TransferTransactionStatus.Rejected;
    }

    public async Task<string> SendTransferTransaction(string to, decimal amount)
    {
        try
        {
            TransactionInput transactionInput;
            lock (this)
            {
                transactionInput = new TransactionInput(TransactionType.EIP1559.AsHexBigInteger(), null, to,
                    _accountAddress,
                    _gasLimitOfTransferTransaction, Web3.Convert.ToWei(amount).ToHexBigInteger(),
                    _maxFeePerGasInWei, _maxPriorityFeePerGasInWei) { Nonce = _currentNonce };

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
            lock (this)
            {
                _currentNonce = currentNonce;
            }

            throw;
        }
    }

    public async Task<string> SendTransferTransaction(string to, decimal amount, decimal fee)
    {
        var maxFeePerGasInWei = Web3.Convert.ToWei(fee) / _gasLimitOfTransferTransaction;
        var baseFeeInWei = (await _web3.Eth.GasPrice.SendRequestAsync()).Value * 2;
        var maxPriorityFeePerGasInWei = maxFeePerGasInWei - baseFeeInWei;

        try
        {
            TransactionInput transactionInput;
            lock (this)
            {
                transactionInput = new TransactionInput(TransactionType.EIP1559.AsHexBigInteger(), null, to,
                        _accountAddress,
                        _gasLimitOfTransferTransaction, Web3.Convert.ToWei(amount).ToHexBigInteger(),
                        maxFeePerGasInWei.ToHexBigInteger(), maxPriorityFeePerGasInWei.ToHexBigInteger())
                    { Nonce = _currentNonce };

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
            lock (this)
            {
                _currentNonce = currentNonce;
            }

            throw;
        }
    }

    public async Task RefreshTransferTransactionFee(DateTime refreshTime, int msUntilNextRefresh)
    {
        var baseFeeInWei = (await _web3.Eth.GasPrice.SendRequestAsync()).Value * 2;
        var maxFeePerGasInWei = baseFeeInWei + _maxPriorityFeePerGasInWei.Value;

        _maxFeePerGasInWei = maxFeePerGasInWei.ToHexBigInteger();
        _transferTransactionFeeInEth =
            Web3.Convert.FromWei(maxFeePerGasInWei * _gasLimitOfTransferTransaction.Value);
        _expectedNextUpdate = refreshTime + TimeSpan.FromMilliseconds(msUntilNextRefresh);
    }
    
    public (decimal Value, int TimeToUpdateInMs) GetTransferTransactionFee(DateTime getTime)
    {
        var timeToUpdateInMs = (int)(_expectedNextUpdate - getTime).TotalMilliseconds;

        return (_transferTransactionFeeInEth, timeToUpdateInMs < 0 ? 0 : timeToUpdateInMs);
    }

    private HexBigInteger _maxFeePerGasInWei = null!;

    private decimal _transferTransactionFeeInEth;

    private readonly HexBigInteger _maxPriorityFeePerGasInWei =
        Web3.Convert.ToWei(2, UnitConversion.EthUnit.Gwei).ToHexBigInteger();
    
    private DateTime _expectedNextUpdate;

    private readonly HexBigInteger _gasLimitOfTransferTransaction = new(21_000);

    private readonly Web3 _web3;

    private readonly string _accountAddress;

    private HexBigInteger _currentNonce;
}