using System.Timers;
using System.Transactions;
using Core.Domain.Constants;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace Core.Application;

public class OrderTransferTransactionTracker
{
    public OrderTransferTransactionTracker(IBlockchain blockchain, IUnitOfWork unitOfWork,
        ILogger<OrderTransferTransactionTracker> logger)
    {
        _blockchain = blockchain;
        _unitOfWork = unitOfWork;
        _logger = logger;

        _trackedOrders =
        [
            ..unitOfWork.Repository.GetAll<SellOrder>(o =>
                    o.Status == OrderStatus.Created || o.Status == OrderStatus.ReceiptFiatFromBuyerConfirmedBySeller)
                .GetAwaiter().GetResult()
        ];
        
        _synchronizer = 0;

        _timer = new Timer { AutoReset = true, Enabled = false, Interval = 1000 };
        _timer.Elapsed += Handler;
        _timer.Start();
    }

    public void Track(SellOrder order)
    {
        Interlocked.Increment(ref _synchronizer);
        
        while (_synchronizer != 1)
            Thread.Yield();
        
        _trackedOrders.Add(order);
        
        Interlocked.Decrement(ref _synchronizer);
        
        _logger.Information("Transaction is being tracked. Order: {OrderGuid}; Transaction: {TransactionHashOfOrder}.", 
            order.Guid, order.TransactionHash);
    }
    
    private void ExecuteConcurrently(Action action)
    {
        Interlocked.Increment(ref _synchronizer);

        while (_synchronizer != 1)
            Thread.Yield();

        action();

        Interlocked.Decrement(ref _synchronizer);
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
    
    private async Task ExecuteConcurrently(Func<Task> action)
    {
        Interlocked.Increment(ref _synchronizer);

        while (_synchronizer != 1)
            await Task.Yield();

        await action();

        Interlocked.Decrement(ref _synchronizer);
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
    
    private readonly IBlockchain _blockchain;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<OrderTransferTransactionTracker> _logger;
    
    private readonly HashSet<SellOrder> _trackedOrders;
    
    private int _synchronizer;

    private readonly Timer _timer;
}