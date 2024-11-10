namespace Core.Models;

public class Trader : ModelBase
{
    public required string Name { get; init; }
    
    public required IEnumerable<OrderBase> Orders { get; init; }
    
    public required double SellerRating { get; init; }
    
    public required double BuyerRating { get; init; }
    
    public required string WalletAddress { get; init; }
}