using Core.Application.Models;

namespace Core.Application.Interfaces;

public interface IBlockchain
{
    Task<decimal> GetTransferTransactionFee();
    
    Task<TransferTransactionInfo?> TryGetTransferTransactionInfo(string transactionHash);
    
    Task<string> SendTransferTransaction(string to, decimal amount);

    string ExchangerAccountAddress { get; }
}