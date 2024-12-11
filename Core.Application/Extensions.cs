using Core.Application.Commands;
using Core.Application.PipelineBehaviors;
using Core.Domain.Interfaces;
using Core.Domain.Services;
using FluentResults;
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
            cfg.AddBehavior<IPipelineBehavior<GetTransferTransactionFeeCommand, Result<(decimal Value, double TimeToUpdateInMs)>>, LoggingBehavior<(decimal Value, double TimeToUpdateInMs)>>();
        });

        return services;
    }
}