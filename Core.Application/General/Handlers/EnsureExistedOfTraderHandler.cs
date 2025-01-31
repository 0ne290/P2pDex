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
        if (await _unitOfWork.Repository.Exists<Trader>(t => t.Id == request.Id))
            return new CommandResult(new { message = "Trader is already exists." });
        
        var trader = new Trader(request.Id);
        
        await _unitOfWork.Repository.Add(trader);
        await _unitOfWork.SaveAllTrackedEntities();
        
        return new CommandResult(new { message = "Trader is created." });
    }
    
    private readonly IUnitOfWork _unitOfWork;
}