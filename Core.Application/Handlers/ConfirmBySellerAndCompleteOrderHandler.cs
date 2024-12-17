using Core.Application.Commands;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.Handlers;

public class ConfirmBySellerAndCompleteOrderHandler : IRequestHandler<ConfirmBySellerAndCompleteOrderCommand, CommandResult>
{
    public ConfirmBySellerAndCompleteOrderHandler(IUnitOfWork unitOfWork, IBlockchain blockchain)
    {
        _unitOfWork = unitOfWork;
        _blockchain = blockchain;
    }
    
    public async Task<CommandResult> Handle(ConfirmBySellerAndCompleteOrderCommand request, CancellationToken _)
    {
        var order = await _unitOfWork.Repository.TryGetByGuid<SellOrder>(request.OrderGuid);

        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        
        order.ConfirmBySeller();

        if (!Equals(order.SellerGuid, request.SellerGuid))
            throw new InvariantViolationException("Trader is not a buyer.");

        var transactionHash = await _blockchain.SendTransaction(order.BuyerWalletAddress, order.CryptoAmount);
        order.Complete(transactionHash);
        
        await _unitOfWork.Save();
        
        return new CommandResult(new { guid = order.Guid, status = order.Status });
    }

    private readonly IUnitOfWork _unitOfWork;

    private readonly IBlockchain _blockchain;
}
