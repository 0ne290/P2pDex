using Core.Application.Models;
using Core.Domain.Constants;

namespace Core.Application.Interfaces;

public interface IBlockchain
{
    Task<TransferTransaction?> TryGetTransactionByHash(string transactionHash);

    Task<TransferTransactionStatus?> TryGetTransactionStatus(string transactionHash);
    
    Task<string> SendTransferTransaction(string to, decimal amount);
    
    Task<string> SendTransferTransaction(string to, decimal amount, decimal fee);
    
    (decimal Value, double TimeToUpdateInMs) TransferTransactionFee { get; }
}