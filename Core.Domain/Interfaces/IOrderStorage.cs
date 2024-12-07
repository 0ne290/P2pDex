using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

public interface IOrderStorage
{
    Task Add(SellOrder sellOrder);

    Task<SellOrder?> TryGetByGuid(string guid);
    
    Task Update(SellOrder sellOrder);
}