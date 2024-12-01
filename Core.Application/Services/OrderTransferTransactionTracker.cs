using System.Timers;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace Core.Application.Services;

public class OrderTransferTransactionTracker : IDisposable
{
    public OrderTransferTransactionTracker(IBlockchain blockchain, IOrderStorage orderStorage,
        ILogger<OrderTransferTransactionTracker> logger, IEnumerable<OrderBase> trackedOrders, double intervalInMs)
    {
        _blockchain = blockchain;
        _orderStorage = orderStorage;
        _logger = logger;

        _synchronizer = 0;
        _trackedOrders = trackedOrders.ToHashSet();

        _timer = new Timer { AutoReset = true, Enabled = false, Interval = intervalInMs };
        _timer.Elapsed += Handler;

        _timer.Start();
    }

    public void Track(OrderBase order)
    {
        Interlocked.Increment(ref _synchronizer);

        while (_synchronizer != 1)
            Thread.Yield();

        _trackedOrders.Add(order);

        Interlocked.Decrement(ref _synchronizer);

        _logger.LogInformation("Transfer transaction of {@Order} is being tracked.", order);
    }

    private async void Handler(object? sender, ElapsedEventArgs e)
    {
        var updatedOrders = new List<OrderBase>();

        Interlocked.Increment(ref _synchronizer);

        while (_synchronizer != 1)
            Thread.Yield();

        foreach (var order in _trackedOrders)
        {
            if (await _blockchain.GetTransferTransactionStatus(order.TransferTransactionHash!) !=
                TransferTransactionStatus.Confirmed)
                continue;
            
            order.ConfirmByBlockchainOfCryptocurrencyTransferTransaction();
            updatedOrders.Add(order);
            _trackedOrders.Remove(order);
            _logger.LogInformation("Transfer transaction of {@Order} is confirmed.", order);
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
    }

    private readonly IBlockchain _blockchain;

    private readonly IOrderStorage _orderStorage;

    private readonly ILogger<OrderTransferTransactionTracker> _logger;

    private int _synchronizer;

    private readonly HashSet<OrderBase> _trackedOrders;
    
    private readonly Timer _timer;
}