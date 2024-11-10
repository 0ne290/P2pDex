using Core.Enums;

namespace Core.Models;

public class Dispute : ModelBase
{
    public required DisputeStatus Status { get; init; }
    
    public required OrderBase Order { get; init; }
    
    public required string OrderGuid { get; init; }
    
    public required Administrator Administrator { get; init; }
    
    public required string AdministratorGuid { get; init; }
    
    public required string JsonOfChatContent { get; init; }
}