using Core.Domain.Enums;
using Core.Domain.Interfaces;
using Core.Domain.ValueObjects;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;

namespace Infrastructure.Blockchain;

public class EthereumBlockchain : IBlockchain
{
    public EthereumBlockchain(ConcurrentWeb3Wrapper concurrentWeb3Wrapper, string exchangerAccountAddress)
    {
        _concurrentWeb3Wrapper = concurrentWeb3Wrapper;
        ExchangerAccountAddress = exchangerAccountAddress;
    }

    public async Task<decimal> GetTransferTransactionFee()
    {
        var gasPriceInWei = await _concurrentWeb3Wrapper.Execute(async web3 => await web3.Eth.GasPrice.SendRequestAsync());
        
        var gasPriceInEth = Web3.Convert.FromWei(gasPriceInWei);
        
        return gasPriceInEth * GasLimitOfTransferTransaction;
    }

    public async Task<TransferTransactionStatus> GetTransferTransactionStatus(string transactionHash)
    {
        var receipt = await _concurrentWeb3Wrapper.Execute(async web3 =>
            await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash));

        if (receipt == null)
            return TransferTransactionStatus.WaitingConfirmation;
        
        return receipt.Status.Value == 1 ? TransferTransactionStatus.Confirmed : TransferTransactionStatus.Cancelled;
    }

    public async Task<TransferTransactionInfo?> TryGetTransferTransactionInfo(string transactionHash)
    {
        var transaction = await _concurrentWeb3Wrapper.Execute(async web3 =>
            await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash));

        return transaction == null ? null : new TransferTransactionInfo
        {
            From = transaction.From,
            To = transaction.To,
            Amount = Web3.Convert.FromWei(transaction.Value)
        };
    }

    public async Task<string> SendTransferTransaction(string to, decimal amount) =>
        await _concurrentWeb3Wrapper.Execute(async web3 =>
            await web3.TransactionManager.SendTransactionAsync(ExchangerAccountAddress, to,
                Web3.Convert.ToWei(amount).ToHexBigInteger()));
    
    public string ExchangerAccountAddress { get; }
    
    private readonly ConcurrentWeb3Wrapper _concurrentWeb3Wrapper;

    private const decimal GasLimitOfTransferTransaction = 21_000m;
}