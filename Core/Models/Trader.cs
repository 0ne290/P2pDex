namespace Core.Models;

public class Trader : ModelBase
{
    public Trader(string guid, string name,double sellerRating, double buyerRating) : base(guid)
    {
        Name = name;
        SellerRating = sellerRating;
        BuyerRating = buyerRating;
    }

    public string Name { get; }
    
    public double SellerRating { get; }
    
    public double BuyerRating { get; }
}