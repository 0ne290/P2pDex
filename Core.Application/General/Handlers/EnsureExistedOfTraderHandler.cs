using System.Dynamic;
using Core.Application.General.Commands;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.General.Handlers;

public class EnsureExistedOfTraderHandler : IRequestHandler<EnsureExistedOfTraderCommand, CommandResult>
{
    public EnsureExistedOfTraderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CommandResult> Handle(EnsureExistedOfTraderCommand request, CancellationToken _)
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
        
        dynamic ret = new ExpandoObject();
        ret.message = message;
        
        return new CommandResult(ret);
    }
    
    private readonly IUnitOfWork _unitOfWork;
}