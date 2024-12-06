using Core.Domain.ValueObjects;

namespace Core.Application.Interfaces;

public interface IBlockchain
{
    Task<decimal> GetTransferTransactionFee();
    
    Task<TransferTransaction?> TryGetTransferTransactionInfo(string transactionHash);
    
    Task<string> SendTransferTransaction(string to, decimal amount);

    string ExchangerAccountAddress { get; }
}