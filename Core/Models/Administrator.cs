namespace Core.Models;

public class Administrator : ModelBase
{
    public required string Name { get; init; }
    
    public required IEnumerable<Dispute> Disputes { get; init; }
}