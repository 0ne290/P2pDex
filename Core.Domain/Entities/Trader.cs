using Core.Domain.Dtos;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public class Trader : BaseEntity
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

    public static Trader Restore(TraderDto dto)
    {
        if (!Guid.TryParse(dto.Guid, out var guid))
            throw new InvariantViolationException("Guid is invalid.");
        if (dto.CountOfSuccessfulOrdersAsBuyer < 0)
            throw new InvariantViolationException("Count of successful orders as buyer is invalid.");
        if (dto.CountOfSuccessfulOrdersAsSeller < 0)
            throw new InvariantViolationException("Count of successful orders as seller is invalid.");
        if (dto.CountOfWonDisputesAsBuyer < 0)
            throw new InvariantViolationException("Count of won disputes as buyer is invalid.");
        if (dto.CountOfLostDisputesAsBuyer < 0)
            throw new InvariantViolationException("Count of lost disputes as buyer is invalid.");
        if (dto.CountOfWonDisputesAsSeller < 0)
            throw new InvariantViolationException("Count of won disputes as seller is invalid.");
        if (dto.CountOfLostDisputesAsSeller < 0)
            throw new InvariantViolationException("Count of lost disputes as seller is invalid.");

        return new Trader(guid, dto.Name)
        {
            CountOfSuccessfulOrdersAsBuyer = dto.CountOfSuccessfulOrdersAsBuyer,
            CountOfSuccessfulOrdersAsSeller = dto.CountOfSuccessfulOrdersAsSeller,
            CountOfWonDisputesAsBuyer = dto.CountOfWonDisputesAsBuyer,
            CountOfLostDisputesAsBuyer = dto.CountOfLostDisputesAsBuyer,
            CountOfWonDisputesAsSeller = dto.CountOfWonDisputesAsSeller,
            CountOfLostDisputesAsSeller = dto.CountOfLostDisputesAsSeller,
        };
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