using Core.Domain.Models;

namespace Core.Domain.Interfaces;

public interface IBlockchain
{
    Task<TransferTransaction?> TryGetConfirmedTransactionByHash(string transactionHash);
    
    Task<string> SendTransferTransaction(string from, string to, decimal amount);
    
    (decimal Value, double TimeToUpdateInMs) TransferTransactionFee { get; }
}