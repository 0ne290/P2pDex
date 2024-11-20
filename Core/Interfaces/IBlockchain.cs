using Core.Enums;

namespace Core.Interfaces;

public interface IBlockchain
{
    Task<TransactionStatus> GetTransactionStatus(string transactionHash);
}