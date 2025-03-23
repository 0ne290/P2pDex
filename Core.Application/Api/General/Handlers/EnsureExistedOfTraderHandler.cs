using Core.Application.Api.General.Commands;
using Core.Application.Private.Interfaces;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Api.General.Handlers;

public class EnsureExistedOfTraderHandler : IRequestHandler<EnsureExistedOfTraderCommand, EnsureExistedOfTraderResponse>
{
    public EnsureExistedOfTraderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EnsureExistedOfTraderResponse> Handle(EnsureExistedOfTraderCommand request, CancellationToken _)
    {
        var trader = await _unitOfWork.Repository.TryGet<Trader>(t => t.Id == request.Id);
        var message = "Trader already exists and does not require updating.";

        if (trader == null)
        {
            await _unitOfWork.Repository.Add(new Trader(request.Id, request.Name));
            await _unitOfWork.SaveAllTrackedEntities();
            
            message = "Trader is created.";
        }
        else if (trader.Name != request.Name)
        {
            trader.Name = request.Name;
            
            await _unitOfWork.SaveAllTrackedEntities();
            
            message = "Trader is updated.";
        }
        
        return new EnsureExistedOfTraderResponse { Message = message };
    }
    
    private readonly IUnitOfWork _unitOfWork;
}