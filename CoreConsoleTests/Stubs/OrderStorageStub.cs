using Core.Enums;
using Core.Interfaces;
using Core.Models;

namespace CoreConsoleTests.Stubs;

public class OrderStorageStub : IOrderStorage
{
    public OrderStorageStub(IEnumerable<Order> orders)
    {
        _orders = orders.ToList();
    }

    public async Task<ICollection<Order>> GetAllByStatus(OrderStatus status) =>
        await Task.FromResult(_orders.Where(o => o.Status == status).Select(Order.CreateCopy).ToList());

    public Task UpdateAll(IEnumerable<Order> orders)
    {
        throw new NotImplementedException();
    }
    
    private readonly List<Order> _orders;
}