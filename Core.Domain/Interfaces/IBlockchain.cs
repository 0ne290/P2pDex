using Core.Domain.ValueObjects;

namespace Core.Domain.Interfaces;

public interface IBlockchain
{
    Task<TransferTransaction?> TryGetConfirmedTransactionByHash(string transactionHash);
    
    Task<string> SendTransferTransaction(string from, string to, decimal amount);
    
    Task<(decimal Value, double TimeToUpdateInMs)> TransferTransactionFee { get; }
}