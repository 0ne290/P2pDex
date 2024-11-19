using Core.Enums;
using Core.Interfaces;
using Core.Models;

namespace CoreConsoleTests.Stubs;

public class OrderStorageStub : IOrderStorage, IDisposable
{
    public OrderStorageStub(IEnumerable<Order> orders)
    {
        _orders = orders.ToList();
        _logger = Log.Logger;
    }

    public async Task<ICollection<Order>> GetAllByStatus(OrderStatus status) =>
        await Task.FromResult(_orders.Where(o => o.Status == status).Select(Order.CreateCopy).ToList());

    public Task UpdateAll(IEnumerable<Order> orders)
    {
        foreach (var order in orders)
            _orders[_orders.FindIndex(o => o.Guid == order.Guid)] = order;
        
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _logger.Information("OrderStorageStub is disposed.");
    }
    
    private readonly List<Order> _orders;

    private readonly ILogger _logger;
}
