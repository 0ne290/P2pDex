using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Core.Domain.Interfaces;

public interface IOrderStorage
{
    Task Add(Order order);

    Task<Order?> TryGetByGuid(string guid);
    
    Task<ICollection<Order>> GetAllByStatus(OrderStatus status);

    Task UpdateAll(IEnumerable<Order> orders);
    
    Task Update(Order order);
}