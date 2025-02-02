namespace Core.Domain.Entities;

public class Trader
{
    private Trader() { }
    
    public Trader(long id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public long Id { get; private set; }
    
    public string Name { get; private set; }
}