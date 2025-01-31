/*using Core.Application.BuyOrder.Commands;
using Core.Domain.Constants;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.BuyOrder.Handlers;

public class ConfirmReceiptFiatFromBuyerBySellerForBuyOrderHandler : IRequestHandler<ConfirmReceiptFiatFromBuyerBySellerForBuyOrderCommand, CommandResult>
{
    public ConfirmReceiptFiatFromBuyerBySellerForBuyOrderHandler(IUnitOfWork unitOfWork, IBlockchain blockchain)
    {
        _unitOfWork = unitOfWork;
        _blockchain = blockchain;
    }
    
    public async Task<CommandResult> Handle(ConfirmReceiptFiatFromBuyerBySellerForBuyOrderCommand request, CancellationToken _)
    {
        var order = await _unitOfWork.Repository.TryGetByGuid<Domain.Entities.BuyOrder>(request.OrderGuid);

        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        if (!Equals(order.SellerGuid, request.SellerGuid))
            throw new InvariantViolationException("Trader is not a seller.");
        if (order.Status != OrderStatus.TransferFiatToSellerConfirmedByBuyer)
            throw new InvariantViolationException("Order status is invalid.");

        var transactionHash = await _blockchain.SendTransferTransaction(order.BuyerAccountAddress, order.CryptoAmount,
            order.ExchangerToMinersFee);
        order.ConfirmReceiptFiatFromBuyerBySeller(transactionHash);
        
        await _unitOfWork.SaveAllTrackedEntities();
        
        return new CommandResult(new { guid = order.Guid, status = order.Status.ToString() });
    }

    private readonly IUnitOfWork _unitOfWork;

    private readonly IBlockchain _blockchain;
}*/