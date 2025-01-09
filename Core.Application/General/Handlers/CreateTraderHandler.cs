using Core.Application.General.Commands;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.General.Handlers;

public class CreateTraderHandler : IRequestHandler<CreateTraderCommand, CommandResult>
{
    public CreateTraderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CommandResult> Handle(CreateTraderCommand request, CancellationToken _)
    {
        var trader = new Trader(Guid.NewGuid(), request.Name);

        await _unitOfWork.Repository.Add(trader);
        await _unitOfWork.SaveAllTrackedEntities();
        
        return new CommandResult(new { guid = trader.Guid });
    }
    
    private readonly IUnitOfWork _unitOfWork;
}