using Core.Domain.Enums;
using Core.Domain.ValueObjects;

namespace Core.Domain.Interfaces;

public interface IBlockchain
{
    Task<decimal> GetTransferTransactionFee();
    
    Task<TransferTransactionStatus> GetTransferTransactionStatus(string transactionHash);
    
    Task<TransferTransactionInfo?> TryGetTransferTransactionInfo(string transactionHash);
    
    Task<string> SendTransferTransaction(string to, decimal amount);

    string ExchangerAccountAddress { get; }
}