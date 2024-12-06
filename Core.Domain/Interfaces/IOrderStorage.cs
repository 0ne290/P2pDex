using Core.Domain.Entities;

namespace Core.Application.Interfaces;

public interface IOrderStorage
{
    Task Add(SellOrder sellOrder);

    Task<SellOrder?> TryGetByGuid(string guid);
    
    Task Update(SellOrder sellOrder);
}