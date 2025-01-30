namespace Core.Domain.Entities;

public class Trader : BaseEntity
{
    private Trader() { }
    
    public Trader(Guid guid, long id) : base(guid)
    {
        Id = id;
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
    
    public long Id { get; private set; }
    
    public int CountOfSuccessfulOrdersAsBuyer { get; private set; }
    
    public int CountOfSuccessfulOrdersAsSeller { get; private set; }
    
    public int CountOfWonDisputesAsBuyer { get; private set; }
    
    public int CountOfLostDisputesAsBuyer { get; private set; }
    
    public int CountOfWonDisputesAsSeller { get; private set; }
    
    public int CountOfLostDisputesAsSeller { get; private set; }
}