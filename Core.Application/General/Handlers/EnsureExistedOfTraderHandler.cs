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

        if (trader == null)
        {
            await _unitOfWork.Repository.Add(new Trader(request.Id, request.Name));
            await _unitOfWork.SaveAllTrackedEntities();
            
            return new CommandResult(new { message = "Trader is created." });
        }
        if (trader.Name != request.Name)
        {
            trader.Name = request.Name;
            
            await _unitOfWork.SaveAllTrackedEntities();
            
            return new CommandResult(new { message = "Trader is updated." });
        }
        
        return new CommandResult(new { message = "Trader already exists and does not require updating." });
    }
    
    private readonly IUnitOfWork _unitOfWork;
}