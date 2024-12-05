using Entities = Core.Domain.Entities;

namespace Infrastructure.Storage.Models;

public class Trader : ModelBase
{
    public static Trader FromEntity(Entities.Trader entity) => new()
    {
        Guid = entity.Guid.ToString(),
        Name = entity.Name,
        CountOfSuccessfulOrdersAsBuyer = entity.CountOfSuccessfulOrdersAsBuyer,
        CountOfSuccessfulOrdersAsSeller = entity.CountOfSuccessfulOrdersAsSeller,
        CountOfWonDisputesAsBuyer = entity.CountOfWonDisputesAsBuyer,
        CountOfLostDisputesAsBuyer = entity.CountOfLostDisputesAsBuyer,
        CountOfWonDisputesAsSeller = entity.CountOfWonDisputesAsSeller,
        CountOfLostDisputesAsSeller = entity.CountOfLostDisputesAsSeller
    };
    
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Name { get; init; }
    
    public required int CountOfSuccessfulOrdersAsBuyer { get; init; }

    public required int CountOfSuccessfulOrdersAsSeller { get; init; }

    public required int CountOfWonDisputesAsBuyer { get; init; }

    public required int CountOfLostDisputesAsBuyer { get; init; }

    public required int CountOfWonDisputesAsSeller { get; init; }

    public required int CountOfLostDisputesAsSeller { get; init; }
}