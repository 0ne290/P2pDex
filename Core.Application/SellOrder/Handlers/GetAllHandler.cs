using Core.Application.SellOrder.Commands;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.SellOrder.Handlers;

public class GetAllSellOrdersHandler : IRequestHandler<GetAllSellOrdersCommand, CommandResult>
{
    public GetAllSellOrdersHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CommandResult> Handle(GetAllSellOrdersCommand _, CancellationToken __)
    {
        var ret = new Dictionary<string, object>
        {
            ["sellOrders"] = await _unitOfWork.Repository.GetAllSellOrdersBySellers()
        };
        
        return new CommandResult(ret);
    }
    
    private readonly IUnitOfWork _unitOfWork;
}