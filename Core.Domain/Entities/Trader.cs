using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public class Trader : EntityBase
{
    public Trader(Guid guid, string name) : base(guid)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 32)
            throw new InvariantViolationException("Name is invalid.");
        
        Name = name;
        CountOfSuccessfulOrdersAsBuyer = 0;
        CountOfSuccessfulOrdersAsSeller = 0;
        CountOfWonDisputesAsBuyer = 0;
        CountOfLostDisputesAsBuyer = 0;
        CountOfWonDisputesAsSeller = 0;
        CountOfLostDisputesAsSeller = 0;
    }

    public void IncrementSuccessfulOrdersAsBuyer() => CountOfSuccessfulOrdersAsBuyer++;
    
    public void IncrementSuccessfulOrdersAsSeller() => CountOfSuccessfulOrdersAsSeller++;
    
    public void IncrementWonDisputesAsBuyer() => CountOfWonDisputesAsBuyer++;
    
    public void IncrementLostDisputesAsBuyer() => CountOfLostDisputesAsBuyer++;
    
    public void IncrementWonDisputesAsSeller() => CountOfWonDisputesAsSeller++;
    
    public void IncrementLostDisputesAsSeller() => CountOfLostDisputesAsSeller++;

    public string Name { get; }
    
    public int CountOfSuccessfulOrdersAsBuyer { get; private set; }
    
    public int CountOfSuccessfulOrdersAsSeller { get; private set; }
    
    public int CountOfWonDisputesAsBuyer { get; private set; }
    
    public int CountOfLostDisputesAsBuyer { get; private set; }
    
    public int CountOfWonDisputesAsSeller { get; private set; }
    
    public int CountOfLostDisputesAsSeller { get; private set; }
}