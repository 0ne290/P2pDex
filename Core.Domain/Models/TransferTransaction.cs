namespace Core.Domain.Models;

public class TransferTransaction
{
    public required string From { get; init; }
    
    public required string To { get; init; }
    
    public required decimal Amount { get; init; }
}