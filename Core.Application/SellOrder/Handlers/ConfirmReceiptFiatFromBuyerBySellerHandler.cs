using Core.Application.SellOrder.Commands;
using Core.Domain.Constants;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.SellOrder.Handlers;

public class ConfirmReceiptFiatFromBuyerBySellerForSellOrderHandler : IRequestHandler<ConfirmReceiptFiatFromBuyerBySellerForSellOrderCommand, CommandResult>
{
    public ConfirmReceiptFiatFromBuyerBySellerForSellOrderHandler(IUnitOfWork unitOfWork, IBlockchain blockchain)
    {
        _unitOfWork = unitOfWork;
        _blockchain = blockchain;
    }
    
    public async Task<CommandResult> Handle(ConfirmReceiptFiatFromBuyerBySellerForSellOrderCommand request, CancellationToken _)
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
        
        return new CommandResult(new { guid = order.Guid, status = order.Status.ToString() });
    }

    private readonly IUnitOfWork _unitOfWork;

    private readonly IBlockchain _blockchain;
}