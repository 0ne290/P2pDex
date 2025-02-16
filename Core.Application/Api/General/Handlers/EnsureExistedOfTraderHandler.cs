using Core.Application.Interfaces;
using Core.Application.UseCases.General.Commands;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.UseCases.General.Handlers;

public class EnsureExistedOfTraderHandler : IRequestHandler<EnsureExistedOfTraderCommand, IDictionary<string, object>>
{
    public EnsureExistedOfTraderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IDictionary<string, object>> Handle(EnsureExistedOfTraderCommand request, CancellationToken _)
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
        
        IDictionary<string, object> ret = new Dictionary<string, object>
        {
            ["message"] = message
        };
        
        return ret;
    }
    
    private readonly IUnitOfWork _unitOfWork;
}