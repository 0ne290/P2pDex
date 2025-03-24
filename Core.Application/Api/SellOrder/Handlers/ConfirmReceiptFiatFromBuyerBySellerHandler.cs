using Core.Application.Api.SellOrder.Commands;
using Core.Application.Private.Interfaces;
using Core.Domain.Constants;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;

namespace Core.Application.Api.SellOrder.Handlers;

public class ConfirmReceiptFiatFromBuyerBySellerForSellOrderHandler : IRequestHandler<ConfirmReceiptFiatFromBuyerBySellerForSellOrderCommand, OrderStatusChangeResponse>
{
    public ConfirmReceiptFiatFromBuyerBySellerForSellOrderHandler(IUnitOfWork unitOfWork, IBlockchain blockchain)
    {
        _unitOfWork = unitOfWork;
        _blockchain = blockchain;
    }
    
    public async Task<OrderStatusChangeResponse> Handle(ConfirmReceiptFiatFromBuyerBySellerForSellOrderCommand request, CancellationToken _)
    {
        if (!await _unitOfWork.Repository.Exists<Trader>(t => t.Id == request.SellerId))
            throw new InvariantViolationException("Seller does not exists.");
        
        var order = await _unitOfWork.Repository.TryGet<Domain.Entities.SellOrder>(o => o.Guid.Equals(request.OrderGuid));
        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        if (!Equals(order.SellerId, request.SellerId))
            throw new InvariantViolationException("Trader is not a seller.");
        if (order.Status != OrderStatus.TransferFiatToSellerConfirmedByBuyer)
            throw new InvariantViolationException("Order status is invalid.");

        var transactionHash = await _blockchain.SendTransferTransaction(order.BuyerAccountAddress!, order.CryptoAmount,
            order.ExchangerToMinersFee);
        order.ConfirmReceiptFiatFromBuyerBySeller(transactionHash);
        
        await _unitOfWork.SaveAllTrackedEntities();
        
        return new OrderStatusChangeResponse { Guid = order.Guid, Status = order.Status };
    }

    private readonly IUnitOfWork _unitOfWork;

    private readonly IBlockchain _blockchain;
}