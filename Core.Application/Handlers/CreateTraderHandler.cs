using Core.Application.Commands;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using FluentResults;
using MediatR;

namespace Core.Application.Handlers;

public class CreateTraderHandler : IRequestHandler<CreateTraderCommand, Result<Guid>>
{
    public CreateTraderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateTraderCommand request, CancellationToken _)
    {
        var trader = new Trader(Guid.NewGuid(), request.Name);

        await _unitOfWork.Repository.Add(trader);
        await _unitOfWork.Save();
        
        return Result.Ok(trader.Guid);
    }
    
    private readonly IUnitOfWork _unitOfWork;
}