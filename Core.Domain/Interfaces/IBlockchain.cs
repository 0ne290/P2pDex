using Core.Domain.Enums;

namespace Core.Domain.Interfaces;

public interface IBlockchain
{
    Task<TransactionStatus> GetTransactionStatus(string transactionHash);
    
    Task<string> SendTransaction(string from, string to, string value);
}