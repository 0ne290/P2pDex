namespace Core.Domain.Entities;

public abstract class EntityBase
{
    protected EntityBase(Guid guid) => Guid = guid;
    
    public Guid Guid { get; }
}