using Core.Domain.Entities;

namespace Core.Application.Interfaces;

public interface IOrderStorage
{
    Task Add(Order order);

    Task<Order?> TryGetByGuid(string guid);
    
    Task Update(Order order);
}