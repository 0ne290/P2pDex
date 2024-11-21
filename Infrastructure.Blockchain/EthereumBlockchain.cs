using Core.Enums;
using Core.Interfaces;
using Nethereum.Web3;

namespace Infrastructure.Blockchain;

public class EthereumBlockchain : IBlockchain
{
    public EthereumBlockchain(Web3 web3)
    {
        _web3 = web3;
        _synchronizer = 0;
    }
    
    public async Task<TransactionStatus> GetTransactionStatus(string transactionHash)
    {
        Interlocked.Increment(ref _synchronizer);
        
        while (_synchronizer != 1)
            Thread.Yield();
        
        var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
        
        Interlocked.Decrement(ref _synchronizer);

        if (receipt == null)
            return TransactionStatus.WaitingConfirmation;
        
        return receipt.Status.Value == 1 ? TransactionStatus.Confirmed : TransactionStatus.Cancelled;
    }
    
    private int _synchronizer;

    private readonly Web3 _web3;
}