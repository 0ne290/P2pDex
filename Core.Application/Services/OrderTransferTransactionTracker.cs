using System.Timers;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Interfaces;
using Timer = System.Timers.Timer;

namespace Core.Application.Services;

public class OrderTransferTransactionTracker : IDisposable
{
    public OrderTransferTransactionTracker(IBlockchain blockchain, IOrderStorage orderStorage, IEnumerable<OrderBase> trackedOrders, double intervalInMs)
    {
        _blockchain = blockchain;
        _orderStorage = orderStorage;

        _synchronizer = 0;
        _trackedOrders = trackedOrders.ToHashSet();

        _timer = new Timer { AutoReset = true, Enabled = false, Interval = intervalInMs };
        _timer.Elapsed += Handler;

        _timer.Start();
    }

    public void Track(OrderBase order) => ExecuteConcurrently(() => _trackedOrders.Add(order));
    
    private void ExecuteConcurrently(Action action)
    {
        Interlocked.Increment(ref _synchronizer);

        while (_synchronizer != 1)
            Thread.Yield();

        action();

        Interlocked.Decrement(ref _synchronizer);
    }

    private async void Handler(object? sender, ElapsedEventArgs e) => await ExecuteConcurrently(async () =>
    {
        var updatedOrders = new List<OrderBase>();

        foreach (var order in _trackedOrders)
        {
            if (await _blockchain.GetTransferTransactionStatus(order.TransferTransactionHash!) !=
                TransferTransactionStatus.Confirmed)
                continue;

            order.ConfirmByBlockchainOfCryptocurrencyTransferTransaction();
            updatedOrders.Add(order);
            _trackedOrders.Remove(order);
        }

        if (updatedOrders.Count > 0)
            await _orderStorage.UpdateAll(updatedOrders);
    });

    private async Task ExecuteConcurrently(Func<Task> action)
    {
        Interlocked.Increment(ref _synchronizer);

        while (_synchronizer != 1)
            Thread.Yield();

        await action();

        Interlocked.Decrement(ref _synchronizer);
    }

    public void Dispose()
    {
        _timer.Elapsed -= Handler;
        _timer.Stop();

        Join();

        _timer.Dispose();
    }

    private void Join()
    {
        while (_synchronizer != 0)
            Thread.Yield();
    }

    private readonly IBlockchain _blockchain;

    private readonly IOrderStorage _orderStorage;

    private int _synchronizer;

    private readonly HashSet<OrderBase> _trackedOrders;
    
    private readonly Timer _timer;
}