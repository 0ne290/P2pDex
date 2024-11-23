namespace Core.Domain.Models;

public abstract class EntityBase
{
    protected EntityBase(string guid) => Guid = guid;
    
    public string Guid { get; }
}