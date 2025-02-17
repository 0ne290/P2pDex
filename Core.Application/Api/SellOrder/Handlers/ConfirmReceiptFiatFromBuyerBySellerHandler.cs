using Core.Application.Api.SellOrder.Commands;
using Core.Application.Private.Interfaces;
using Core.Domain.Constants;
using Core.Domain.Exceptions;
using MediatR;

namespace Core.Application.Api.SellOrder.Handlers;

public class ConfirmReceiptFiatFromBuyerBySellerForSellOrderHandler : IRequestHandler<ConfirmReceiptFiatFromBuyerBySellerForSellOrderCommand, IDictionary<string, object>>
{
    public ConfirmReceiptFiatFromBuyerBySellerForSellOrderHandler(IUnitOfWork unitOfWork, IBlockchain blockchain)
    {
        _unitOfWork = unitOfWork;
        _blockchain = blockchain;
    }
    
    public async Task<IDictionary<string, object>> Handle(ConfirmReceiptFiatFromBuyerBySellerForSellOrderCommand request, CancellationToken _)
    {
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
        
        var ret = new Dictionary<string, object>
        {
            ["guid"] = order.Guid,
            ["status"] = order.Status
        };
        
        return ret;
    }

    private readonly IUnitOfWork _unitOfWork;

    private readonly IBlockchain _blockchain;
}