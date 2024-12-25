using System.Timers;
using Core.Domain.Constants;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace Core.Application;

public class OrderTransferTransactionTracker : IDisposable
{
    public OrderTransferTransactionTracker(IBlockchain blockchain, IUnitOfWork unitOfWork,
        ILogger<OrderTransferTransactionTracker> logger, double intervalInMs)
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

        _timer = new Timer { AutoReset = true, Enabled = false, Interval = intervalInMs };
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
            
            var transactionStatus = await _blockchain.TryGetTransactionStatus(transactionHash);
            if (transactionStatus == null)
                continue;
            // TODO: Доработать/переделать систему комиссий.
            // Этот очень мутный кейс возникает в двух условиях:
            // 1. Если продавец указал в команде создания заказа на продажу хэш "херовой" транзакции, которая либо
            // изначально была отменена, либо отменилась уже после вызова команды. Это условие можно и нужно легко
            // протестировать.
            // 2. Если P2P DEX сама создала какую-то "херовую" транзакцию. Это может произойти, если курс комиссии
            // (неважно - базовой или приоритетной - но если я правильно понимаю Ethereum, кейс с базовой комиссией
            // наиболее вероятен, если вообще не единственный) слишком сильно изменился с момента ее назначения заказу
            // (в случае с заказом на продажу это момент создания заказа, а в случае с заказом на покупку это момент
            // отклика продавца).
            // Этот кейс максимально мутный из-за второго условия, т. к. его невозможно воспроизвести и протестировать
            // и на него невозможно выработать какую-то ответную реакцию. Какие могут быть варианты? Обработать админу
            // заказ в частном порядке в обход P2P DEX? Кинуть мэссэдж "Извините, на данный момент комиссия в сети
            // Ethereum слишком высока"? И тот и другой варианты - полная х*%№я - поэтому вероятность такого события
            // должна быть либо равна 0, либо сведена к минимуму, а для этого требуется доработать/переделать систему
            // комиссий
            if (transactionStatus == TransferTransactionStatus.Rejected)
            {
                _logger.LogCritical(
                    "Order transfer transaction is rejected. Order GUID: {OrderGuid}; Transaction hash: {TransactionHash}.",
                    order.Guid, transactionHash);
                
                throw new DevelopmentErrorException($"Order transfer transaction is rejected. Order GUID: {order.Guid}; Transaction hash: {transactionHash}.");
            }
            
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