using Core.Application.Private.Constants;
using Core.Application.Private.Models;

namespace Core.Application.Private.Interfaces;

public interface IBlockchain
{
    Task<TransferTransaction?> TryGetTransactionByHash(string transactionHash);

    Task<TransferTransactionStatus?> TryGetTransactionStatus(string transactionHash);
    
    Task<string> SendTransferTransaction(string to, decimal amount);
    
    Task<string> SendTransferTransaction(string to, decimal amount, decimal fee);

    Task RefreshTransferTransactionFee(DateTime refreshTime, int msUntilNextRefresh);

    (decimal Value, int TimeToUpdateInMs) GetTransferTransactionFee(DateTime getTime);
}