using Core.Application.Enums;

namespace Core.Application.Models;

public class TransferTransactionInfo
{
    public required TransferTransactionStatus Status { get; init; }
    
    public required string Hash { get; init; }
    
    public required string From { get; init; }
    
    public required string To { get; init; }
    
    public required decimal Amount { get; init; }
}