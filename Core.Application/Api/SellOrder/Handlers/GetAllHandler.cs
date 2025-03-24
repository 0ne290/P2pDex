using Core.Application.Api.SellOrder.Commands;
using Core.Application.Private.Interfaces;
using Core.Domain.Constants;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;

namespace Core.Application.Api.SellOrder.Handlers;

public class GetAllSellOrdersHandler : IRequestHandler<GetAllSellOrdersCommand, GetAllSellOrdersResponse>
{
    public GetAllSellOrdersHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetAllSellOrdersResponse> Handle(GetAllSellOrdersCommand request, CancellationToken __)
    {
        if (!await _unitOfWork.Repository.Exists<Trader>(t => t.Id == request.TraderId))
            throw new InvariantViolationException("Trader does not exists.");
        
        return new GetAllSellOrdersResponse {SellOrders = await _unitOfWork.SellOrdersWithSellersQuery.Execute(o => o.Status == OrderStatus.SellerToExchangerTransferTransactionConfirmed)};
    }
    
    private readonly IUnitOfWork _unitOfWork;
}