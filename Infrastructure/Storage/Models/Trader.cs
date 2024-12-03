namespace Infrastructure.Storage.Models;

public class Trader : ModelBase
{
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Name { get; init; }
    
    public required double SellerRating { get; init; }
    
    public required double BuyerRating { get; init; }
}