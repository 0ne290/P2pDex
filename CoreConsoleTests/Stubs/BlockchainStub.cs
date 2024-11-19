using Core.Interfaces;

namespace CoreConsoleTests.Stubs;

public class BlockchainStub : IBlockchain
{
    private class Transaction
    {
        public required string Hash { get; init; }
        
        public required bool IsConfirmed { get; set; }
    }
    
    public BlockchainStub(IEnumerable<(string Hash, bool IsConfirmed)> transactions)
    {
        _transactions = transactions.Select(t => new Transaction
        {
            Hash = t.Hash,
            IsConfirmed = t.IsConfirmed
        }).ToList();
    }

    public void ConfirmTransaction(int index) => _transactions[index].IsConfirmed = true;

    public bool TransactionConfirmed(string transactionHash) =>
        _transactions.Exists(t => t.Hash == transactionHash && t.IsConfirmed);

    private readonly List<Transaction> _transactions;
}