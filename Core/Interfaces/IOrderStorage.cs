using Core.Enums;
using Core.Models;

namespace Core.Interfaces;

public interface IOrderStorage
{
    Task Add(Order order);
    
    Task<ICollection<Order>> GetAllByStatus(OrderStatus status);

    Task UpdateAll(IEnumerable<Order> orders);
}