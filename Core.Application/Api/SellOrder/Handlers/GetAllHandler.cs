using Core.Application.Interfaces;
using Core.Application.UseCases.SellOrder.Commands;
using MediatR;

namespace Core.Application.UseCases.SellOrder.Handlers;

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