/*
using Core.Domain.Enums;
using Core.Domain.Interfaces;
using Serilog;

namespace Tests.Stubs;

public class OrderStorageStub : IOrderStorage, IDisposable
{
    public OrderStorageStub(IEnumerable<BuyOrder> orders)
    {
        _orders = orders.ToList();
        _logger = Log.Logger;
    }

    public Task Add(BuyOrder sellOrder)
    {
        _orders.Add(sellOrder);

        return Task.CompletedTask;
    }

    public ICollection<BuyOrder> GetAll() => _orders.Select(BuyOrder.CreateCopy).ToList();
    
    public async Task<ICollection<BuyOrder>> GetAllByStatus(OrderStatus status) =>
        await Task.FromResult(_orders.Where(o => o.CurrentStatus == status).Select(BuyOrder.CreateCopy).ToList());

    public Task UpdateAll(IEnumerable<BuyOrder> orders)
    {
        foreach (var sellOrder in orders)
            _orders[_orders.FindIndex(o => o.Guid == sellOrder.Guid)] = sellOrder;
        
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _logger.Information("OrderStorageStub is disposed.");
    }
    
    private readonly List<BuyOrder> _orders;

    private readonly ILogger _logger;
}
*/
