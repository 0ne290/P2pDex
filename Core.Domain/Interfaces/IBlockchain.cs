using Core.Domain.Enums;
using Core.Domain.ValueObjects;

namespace Core.Domain.Interfaces;

public interface IBlockchain
{
    Task<TransactionStatus> GetTransactionStatus(string transactionHash);
    
    Task<Transaction?> TryGetTransaction(string transactionHash);
    
    Task<string> SendTransaction(string from, string to, string value);
}