/*
using Core.Application.BuyOrder.Commands;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.BuyOrder.Handlers;

public class RespondToBuyOrderBySellerHandler : IRequestHandler<RespondToBuyOrderBySellerCommand, CommandResult>
{
    public RespondToBuyOrderBySellerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<CommandResult> Handle(RespondToBuyOrderBySellerCommand request, CancellationToken _)
    {
        var order = await _unitOfWork.Repository.TryGetByGuid<Domain.Entities.BuyOrder>(request.OrderGuid);

        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        
        order.RespondBySeller(request.SellerGuid, request.TransferTransactionHash);
        
        if (!await _unitOfWork.Repository.Exists<Trader>(t => t.Guid.Equals(request.SellerGuid)))
            throw new InvariantViolationException("Seller does not exists.");
        if (await _unitOfWork.Repository.Exists<Domain.Entities.SellOrder>(o =>
                o.SellerToExchangerTransferTransactionHash == request.TransferTransactionHash) ||
            await _unitOfWork.Repository.Exists<Domain.Entities.BuyOrder>(o =>
                o.SellerToExchangerTransferTransactionHash == request.TransferTransactionHash))
            throw new InvariantViolationException("Transaction has already been used to pay for the order.");
    
        await _unitOfWork.SaveAllTrackedEntities();

        return new CommandResult(new { guid = order.Guid, status = order.Status.ToString() });
    }

    private readonly IUnitOfWork _unitOfWork;
}
*/
