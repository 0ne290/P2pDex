using Core.Domain.Enums;

namespace Core.Domain.ValueObjects;

public class TransferTransaction
{
    public required Cryptocurrency Crypto { get; init; }
    
    public required string Hash { get; init; }
    
    public required string From { get; init; }
    
    public required string To { get; init; }
    
    public required decimal Amount { get; init; }
}