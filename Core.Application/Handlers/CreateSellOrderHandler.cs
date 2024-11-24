using Core.Application.Commands;
using FluentValidation.Results;

namespace Core.Application.Handlers;

public class CreateSellOrderHandler
{
    public CreateSellOrderHandler()
    {
        
    }
    
    public async Task Handle(CreateSellOrderCommand request)
    {
        var c = new ValidationResult
        var trader
        var order = Order.CreateSellOrder(Guid.NewGuid().ToString(), )
    }
}