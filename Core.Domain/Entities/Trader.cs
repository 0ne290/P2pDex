namespace Core.Domain.Entities;

public class Trader : EntityBase
{
    public Trader(string guid, string name) : base(guid)
    {
        Name = name;
        SellerRating = 0;
        BuyerRating = 0;
    }

    public string Name { get; }
    
    public double SellerRating { get; }
    
    public double BuyerRating { get; }
}