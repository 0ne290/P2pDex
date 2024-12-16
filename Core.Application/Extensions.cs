using Core.Application.Commands;
using Core.Domain.Interfaces;
using Core.Domain.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, decimal feeRate)
    {
        services.AddScoped<Exchanger>(sp => new Exchanger(sp.GetRequiredService<IBlockchain>(),
            sp.GetRequiredService<IRepository>(), feeRate));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateSellOrderCommand).Assembly);
            
            cfg.AddBehavior<IPipelineBehavior<CalculateFinalCryptoAmountForTransferCommand, CommandResult>,
                LoggingBehavior<CalculateFinalCryptoAmountForTransferCommand, CommandResult>>();
            
            cfg.AddBehavior<IPipelineBehavior<CreateTraderCommand, CommandResult>,
                LoggingBehavior<CreateTraderCommand, CommandResult>>();
            
            cfg.AddBehavior<IPipelineBehavior<CreateSellOrderCommand, CommandResult>,
                LoggingBehavior<CreateSellOrderCommand, CommandResult>>();
        });

        return services;
    }
}