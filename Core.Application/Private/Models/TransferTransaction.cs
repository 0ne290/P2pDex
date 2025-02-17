using Core.Application.Private.Constants;

namespace Core.Application.Private.Models;

public class TransferTransaction
{
    public required TransferTransactionStatus Status { get; init; }
    
    public required string From { get; init; }
    
    public required string To { get; init; }
    
    public required decimal Amount { get; init; }
}