using Core.Application.Commands;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.Handlers;

public class ConfirmOrderBySellerHandler : IRequestHandler<ConfirmOrderBySellerCommand, CommandResult>
{
    public ConfirmOrderBySellerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<CommandResult> Handle(ConfirmOrderBySellerCommand request, CancellationToken _)
    {
        var order = await _unitOfWork.Repository.TryGetByGuid<SellOrder>(request.OrderGuid);

        if (order == null)
            throw new InvariantViolationException("Order does not exists.");
        
        order.ConfirmBySeller();

        if (!Equals(order.SellerGuid, request.SellerGuid))
            throw new InvariantViolationException("Trader is not a buyer.");
        
        await _unitOfWork.Save();
        
        return new CommandResult(new { guid = order.Guid, status = order.Status });
    }

    private readonly IUnitOfWork _unitOfWork;
}