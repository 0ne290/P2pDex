using System.Timers;
using Core.Domain.Constants;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace Core.Application;

public class OrderTransferTransactionTracker : IDisposable
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
        ];
        
        _synchronizer = 0;

        _timer = new Timer { AutoReset = true, Enabled = false, Interval = 1000 };
        _timer.Elapsed += Handler;
        _timer.Start();
    }

    public void Track(SellOrder order) => ExecuteConcurrently(() =>
    {
        _trackedOrders.Add(order);

        var transactionHash = order.Status == OrderStatus.Created
            ? order.SellerToExchangerTransferTransactionHash
            : order.ExchangerToBuyerTransferTransactionHash!;

        _logger.LogInformation(
            "Order transfer transaction is being tracked. Order GUID: {OrderGuid}; Transaction hash: {TransactionHash}.",
            order.Guid, transactionHash);
    });
    
    private void ExecuteConcurrently(Action action)
    {
        Interlocked.Increment(ref _synchronizer);

        while (_synchronizer != 1)
            Thread.Yield();

        action();

        Interlocked.Decrement(ref _synchronizer);
    }

    private async void Handler(object? _, ElapsedEventArgs __) => await ExecuteConcurrentlyAsync(async () =>
    {
        var updatedOrders = new List<SellOrder>();

        foreach (var order in _trackedOrders)
        {
            string transactionHash;
            Action confirmTransaction;
            if (order.Status == OrderStatus.Created)
            {
                transactionHash = order.SellerToExchangerTransferTransactionHash;
                confirmTransaction = order.ConfirmSellerToExchangerTransferTransaction;
            }
            else
            {
                transactionHash = order.ExchangerToBuyerTransferTransactionHash!;
                confirmTransaction = order.ConfirmExchangerToBuyerTransferTransaction;
            }

            if (!await _blockchain.TransactionIsConfirmed(transactionHash))
                continue;
            
            confirmTransaction();
            updatedOrders.Add(order);
            _trackedOrders.Remove(order);
            _logger.LogInformation(
                "Order transfer transaction is confirmed. Order GUID: {OrderGuid}; Transaction hash: {TransactionHash}.",
                order.Guid, transactionHash);
        }
        
        if (updatedOrders.Count > 0)
        {
            _unitOfWork.Repository.UpdateRange(updatedOrders);
            await _unitOfWork.SaveAllTrackedEntities();
            _unitOfWork.UntrackAllEntities();
        }
    });
    
    private async Task ExecuteConcurrentlyAsync(Func<Task> action)
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
        
        Join();
        
        _timer.Dispose();
    }
    
    private void Join()
    {
        while (_synchronizer != 0)
            Thread.Yield();
    }
    
    private readonly IBlockchain _blockchain;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<OrderTransferTransactionTracker> _logger;
    
    private readonly HashSet<SellOrder> _trackedOrders;
    
    private int _synchronizer;

    private readonly Timer _timer;
}