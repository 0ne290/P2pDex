using Core.Domain.Constants;
using Core.Domain.Models;

namespace Core.Domain.Interfaces;

public interface IBlockchain
{
    Task<TransferTransaction?> TryGetConfirmedTransactionByHash(string transactionHash);

    Task<TransferTransactionStatus?> TryGetTransactionStatus(string transactionHash);
    
    Task<string> SendTransferTransaction(string to, decimal amount);
    
    Task<string> SendTransferTransaction(string to, decimal amount, decimal fee);
    
    (decimal Value, double TimeToUpdateInMs) TransferTransactionFee { get; }
}