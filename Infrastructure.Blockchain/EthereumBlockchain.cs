using Core.Enums;
using Core.Interfaces;
using Nethereum.Web3;

namespace Infrastructure.Blockchain;

public class EthereumBlockchain : IBlockchain
{
    public EthereumBlockchain(Web3 web3)
    {
        _web3 = web3;
    }
    
    public async Task<TransactionStatus> GetTransactionStatus(string transactionHash)
    {
        var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

        if (receipt == null)
            return TransactionStatus.WaitingConfirmation;
        
        return receipt.Status.Value == 1 ? TransactionStatus.Confirmed : TransactionStatus.Cancelled;
    }

    private readonly Web3 _web3;
}