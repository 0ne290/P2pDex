namespace Core.Domain.Entities;

public abstract class BaseEntity
{
    protected BaseEntity() { }
    
    protected BaseEntity(Guid guid) => Guid = guid;
    
    public Guid Guid { get; private set; }
}