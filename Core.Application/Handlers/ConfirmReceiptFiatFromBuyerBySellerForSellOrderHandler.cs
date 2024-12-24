using Core.Application.Commands;
using Core.Domain.Constants;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.Handlers;

public class ConfirmReceiptFiatFromBuyerBySellerForSellOrderHandler : IRequestHandler<ConfirmBySellerAndCompleteOrderCommand, CommandResult>
{
    public ConfirmReceiptFiatFromBuyerBySellerForSellOrderHandler(IUnitOfWork unitOfWork, IBlockchain blockchain, OrderTransferTransactionTracker orderTransferTransactionTracker)
    {
        _unitOfWork = unitOfWork;
        _blockchain = blockchain;
        _orderTransferTransactionTracker = orderTransferTransactionTracker;
    }
    
    public async Task<CommandResult> Handle(ConfirmBySellerAndCompleteOrderCommand request, CancellationToken _)
    {
        var order = await _unitOfWork.Repository.TryGetByGuid<SellOrder>(request.OrderGuid);

        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        if (!Equals(order.SellerGuid, request.SellerGuid))
            throw new InvariantViolationException("Trader is not a seller.");
        if (order.Status != OrderStatus.TransferFiatToSellerConfirmedByBuyer)
            throw new InvariantViolationException("Order status is invalid.");
        
        var transactionHash = await _blockchain.SendTransferTransaction(order.BuyerAccountAddress!, order.CryptoAmount);
        order.ConfirmReceiptFiatFromBuyerBySeller(transactionHash);
        
        await _unitOfWork.SaveAllTrackedEntities();
        
        _orderTransferTransactionTracker.Track(order);
        
        return new CommandResult(new { guid = order.Guid, status = order.Status.ToString() });
    }

    private readonly IUnitOfWork _unitOfWork;

    private readonly IBlockchain _blockchain;

    private readonly OrderTransferTransactionTracker _orderTransferTransactionTracker;
}