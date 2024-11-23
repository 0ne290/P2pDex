using System.Timers;
using Core.Domain.Enums;
using Core.Domain.Interfaces;
using Core.Domain.Models;
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
        _synchronizer = 0;
        _trackedOrders = trackedOrders.ToHashSet();
        _blockchain = blockchain;
        _orderStorage = orderStorage;
        _logger = Log.Logger;
        
        _timer.Start();
    }

    public void TrackOrder(Order order)
    {
        Interlocked.Increment(ref _synchronizer);
        
        while (_synchronizer != 1)
            Thread.Yield();
        
        _trackedOrders.Add(order);
        
        Interlocked.Decrement(ref _synchronizer);
        
        _logger.Information("Transaction is being tracked. Order: {OrderGuid}; Transaction: {TransactionHashOfOrder}.", 
            order.Guid, order.TransactionHash);
    }

    private async void Handler(object? sender, ElapsedEventArgs e)
    {
        var updatedOrders = new List<Order>();
        
        Interlocked.Increment(ref _synchronizer);
        
        while (_synchronizer != 1)
            Thread.Yield();

        foreach (var order in _trackedOrders)
        {
            if (await _blockchain.GetTransactionStatus(order.TransactionHash!) == TransactionStatus.Confirmed)
            {
                order.ConfirmTransaction();
                updatedOrders.Add(order);
                _trackedOrders.Remove(order);
                _logger.Information(
                    "Transaction is confirmed. Order: {OrderGuid}; Transaction: {TransactionHashOfOrder}.",
                    order.Guid, order.TransactionHash);
            }


        }

        Interlocked.Decrement(ref _synchronizer);
        
        if (updatedOrders.Count > 0) 
            await _orderStorage.UpdateAll(updatedOrders);
    }

    public void Dispose()
    {
        _timer.Elapsed -= Handler;
        _timer.Stop();
        
        while (_synchronizer != 0)
            Thread.Yield();
        
        _timer.Dispose();
        
        _logger.Information("OrderTracker is disposed.");
    }

    private readonly Timer _timer;

    private int _synchronizer;

    private readonly HashSet<Order> _trackedOrders;

    private readonly IBlockchain _blockchain;

    private readonly IOrderStorage _orderStorage;

    private readonly ILogger _logger;
}