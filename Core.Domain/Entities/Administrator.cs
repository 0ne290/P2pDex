using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public class Administrator : BaseEntity
{
    public Administrator(Guid guid, string name) : base(guid)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 32)
            throw new InvariantViolationException("Name is invalid.");

        Name = name;
        CountOfDisputeResolved = 0;
    }

    public void IncrementDisputeResolved() => CountOfDisputeResolved++;
    
    public string Name { get; }
    
    public int CountOfDisputeResolved { get; private set; }
}