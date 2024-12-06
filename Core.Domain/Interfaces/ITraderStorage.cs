using Core.Domain.Entities;

namespace Core.Application.Interfaces;

public interface ITraderStorage
{
    Task<Trader?> TryGetByGuid(string guid);
}