using System.Timers;
using Core.Interfaces;
using Core.Models;
using Serilog;
using ILogger = Serilog.ILogger;
using Timer = System.Timers.Timer;

namespace CoreConsoleTests;

public class OrderTracker : IDisposable
{
    public OrderTracker(IEnumerable<Order> trackedOrders, IBlockchain blockchain, IOrderStorage orderStorage)
    {
        _timer = new Timer { AutoReset = true, Enabled = false, Interval = 5000 };
        _timer.Elapsed += Handler;
        _numberOfAtiveHandlers = 0;
        _trackedOrders = trackedOrders.ToHashSet();
        _locker = new object();
        _blockchain = blockchain;
        _orderStorage = orderStorage;
        _logger = Log.Logger;
        
        _timer.Start();
    }

    public void TrackOrder(Order order)
    {
        lock (_locker)
        {
            _trackedOrders.Add(order);
            _logger.Information("Transaction is being tracked. Order: {OrderGuid}; Transaction: {TransactionHashOfOrder}.",
                order.Guid, order.TransactionHash);
        }
    }

    private async void Handler(object? sender, ElapsedEventArgs e)
    {
        Interlocked.Increment(ref _numberOfAtiveHandlers);
        
        var updatedOrders = new List<Order>();

        lock (_locker)
        {
            foreach (var order in _trackedOrders.Where(order => _blockchain.TransactionConfirmed(order.TransactionHash!)))
            {
                order.ConfirmTransaction();
                updatedOrders.Add(order);
                _trackedOrders.Remove(order);
                _logger.Information("Transaction is confirmed. Order: {OrderGuid}; Transaction: {TransactionHashOfOrder}.",
                    order.Guid, order.TransactionHash);
            }
        }

        if (updatedOrders.Count > 0)
            await _orderStorage.UpdateAll(updatedOrders);
        
        Interlocked.Decrement(ref _numberOfAtiveHandlers);
    }

    public void Dispose()
    {
        _timer.Elapsed -= Handler;
        _timer.Stop();
        
        while (_numberOfAtiveHandlers != 0)
            Thread.Yield();
        
        _timer.Dispose();
        
        _logger.Information("OrderTracker is disposed.");
    }

    private readonly Timer _timer;

    private int _numberOfAtiveHandlers;

    private readonly HashSet<Order> _trackedOrders;

    private readonly object _locker;

    private readonly IBlockchain _blockchain;

    private readonly IOrderStorage _orderStorage;

    private readonly ILogger _logger;
}
