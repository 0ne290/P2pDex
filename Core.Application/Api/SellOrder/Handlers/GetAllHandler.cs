using Core.Application.Api.SellOrder.Commands;
using Core.Application.Private.Interfaces;
using Core.Domain.Constants;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;

namespace Core.Application.Api.SellOrder.Handlers;

public class GetAllSellOrdersHandler : IRequestHandler<GetAllSellOrdersCommand, IDictionary<string, object>>
{
    public GetAllSellOrdersHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IDictionary<string, object>> Handle(GetAllSellOrdersCommand request, CancellationToken __)
    {
        if (!await _unitOfWork.Repository.Exists<Trader>(t => t.Id == request.TraderId))
            throw new InvariantViolationException("Trader does not exists.");
        
        IDictionary<string, object> ret = new Dictionary<string, object>
        {
            ["sellOrders"] = await _unitOfWork.SellOrdersAndTheirSellersQuery.Execute(o =>
                o.Status == OrderStatus.SellerToExchangerTransferTransactionConfirmed)
        };
        
        return ret;
    }
    
    private readonly IUnitOfWork _unitOfWork;
}