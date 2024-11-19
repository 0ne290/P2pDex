using Core.Enums;
using Core.Models;

namespace Core.Interfaces;

public interface IOrderStorage
{
    Task<ICollection<Order>> GetAllByStatus(OrderStatus status);

    Task UpdateAll(IEnumerable<Order> orders);
}