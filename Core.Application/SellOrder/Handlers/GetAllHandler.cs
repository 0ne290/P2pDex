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

    public async Task<CommandResult> Handle(GetAllSellOrdersCommand _, CancellationToken __) =>
        new(new { sellOrders = await _unitOfWork.Repository.GetAllSellOrdersBySellers() });
    
    private readonly IUnitOfWork _unitOfWork;
}