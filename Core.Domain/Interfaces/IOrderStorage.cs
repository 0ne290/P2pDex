using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Core.Domain.Interfaces;

public interface IOrderStorage
{
    Task Add(OrderBase order);

    Task<OrderBase?> TryGetByGuid(string guid);
    
    Task<ICollection<OrderBase>> GetAllByStatus(OrderStatus status);

    Task UpdateAll(IEnumerable<OrderBase> orders);
    
    Task Update(OrderBase order);
}