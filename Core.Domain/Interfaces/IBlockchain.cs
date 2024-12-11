using Core.Domain.Models;

namespace Core.Domain.Interfaces;

public interface IBlockchain
{
    Task<TransferTransaction?> TryGetConfirmedTransactionByHash(string transactionHash);
    
    Task<string> SendTransferTransaction(string to, decimal amount);
    
    string AccountAddress { get; }
    
    (decimal Value, double TimeToUpdateInMs) TransferTransactionFee { get; }
}