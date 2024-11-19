namespace Core.Interfaces;

public interface IBlockchain
{
    bool TransactionConfirmed(string transactionHash);
}