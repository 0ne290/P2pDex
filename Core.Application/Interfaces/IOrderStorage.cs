using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Core.Application.Interfaces;

public interface IOrderStorage
{
    Task Add(OrderBase order);

    Task<OrderBase?> TryGetByGuid(string guid);
    
    Task<ICollection<Order>> GetAllByStatus(OrderStatus status);

    Task UpdateAll(IEnumerable<Order> orders);
    
    Task Update(OrderBase order);
}