using Core.Domain.Enums;
using Core.Domain.Interfaces;
using Serilog;

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

    public void AddTransaction(string hash) => _transactions.Add(new Transaction { Hash = hash, IsConfirmed = false });

    public void ConfirmTransaction(int index) => _transactions[index].IsConfirmed = true;

    public Task<TransactionStatus> GetTransactionStatus(string transactionHash) => Task.FromResult(
        _transactions.Exists(t => t.Hash == transactionHash && t.IsConfirmed)
            ? TransactionStatus.Confirmed
            : TransactionStatus.WaitingConfirmation);

    public void Dispose()
    {
        _logger.Information("BlockchainStub is disposed.");
    }

    private readonly List<Transaction> _transactions;

    private readonly ILogger _logger;
}
