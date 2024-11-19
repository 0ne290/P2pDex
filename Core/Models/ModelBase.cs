namespace Core.Models;

public abstract class ModelBase
{
    protected ModelBase(string guid) => Guid = guid;
    
    public string Guid { get; }
}