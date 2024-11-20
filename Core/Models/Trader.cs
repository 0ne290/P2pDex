namespace Core.Models;

public class Trader : ModelBase
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
