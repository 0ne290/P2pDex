using Core.Interfaces;

namespace CoreConsoleTests.Stubs;

public class BlockchainStub : IBlockchain, IDisposable
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
        _logger = Log.Logger;
    }

    public void ConfirmTransaction(int index) => _transactions[index].IsConfirmed = true;

    public bool TransactionConfirmed(string transactionHash) =>
        _transactions.Exists(t => t.Hash == transactionHash && t.IsConfirmed);

    public void Dispose()
    {
        _logger.Information("BlockchainStub is disposed.");
    }

    private readonly List<Transaction> _transactions;

    private readonly ILogger _logger;
}
