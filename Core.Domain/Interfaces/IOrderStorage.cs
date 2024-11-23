using Core.Domain.Enums;
using Core.Domain.Models;

namespace Core.Domain.Interfaces;

public interface IOrderStorage
{
    Task Add(Order order);
    
    Task<ICollection<Order>> GetAllByStatus(OrderStatus status);

    Task UpdateAll(IEnumerable<Order> orders);
}