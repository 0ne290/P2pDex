using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

public interface ITraderStorage
{
    Task<Trader?> TryGetByGuid(Guid guid);
}