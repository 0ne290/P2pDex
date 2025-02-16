using Core.Application.Interfaces;
using Core.Application.UseCases.SellOrder.Commands;
using Core.Domain.Constants;
using Core.Domain.Exceptions;
using MediatR;

namespace Core.Application.UseCases.SellOrder.Handlers;

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
        
        var ret = new Dictionary<string, object>
        {
            ["guid"] = order.Guid,
            ["status"] = order.Status.ToString()
        };
        
        return new CommandResult(ret);
    }

    private readonly IUnitOfWork _unitOfWork;

    private readonly IBlockchain _blockchain;
}