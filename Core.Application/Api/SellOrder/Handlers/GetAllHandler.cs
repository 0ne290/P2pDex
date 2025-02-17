using Core.Application.Api.SellOrder.Commands;
using Core.Application.Private.Interfaces;
using MediatR;

namespace Core.Application.Api.SellOrder.Handlers;

public class GetAllSellOrdersHandler : IRequestHandler<GetAllSellOrdersCommand, IDictionary<string, object>>
{
    public GetAllSellOrdersHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IDictionary<string, object>> Handle(GetAllSellOrdersCommand _, CancellationToken __)
    {
        IDictionary<string, object> ret = new Dictionary<string, object>
        {
            ["sellOrders"] = await _unitOfWork.Repository.GetAllSellOrdersBySellers()
        };
        
        return ret;
    }
    
    private readonly IUnitOfWork _unitOfWork;
}