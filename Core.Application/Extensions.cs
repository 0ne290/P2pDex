using Core.Application.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, Func<IServiceProvider, ExchangerConfiguration> exchangerConfigurationFactory)
    {
        services.AddSingleton(exchangerConfigurationFactory);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateSellOrderCommand).Assembly);

            cfg.AddBehavior<IPipelineBehavior<CalculateFinalCryptoAmountForTransferCommand, CommandResult>,
                LoggingBehavior<CalculateFinalCryptoAmountForTransferCommand, CommandResult>>();
            
            cfg.AddBehavior<IPipelineBehavior<GetExchangerAccountAddressCommand, CommandResult>,
                LoggingBehavior<GetExchangerAccountAddressCommand, CommandResult>>();

            cfg.AddBehavior<IPipelineBehavior<CreateTraderCommand, CommandResult>,
                LoggingBehavior<CreateTraderCommand, CommandResult>>();

            cfg.AddBehavior<IPipelineBehavior<CreateSellOrderCommand, CommandResult>,
                LoggingBehavior<CreateSellOrderCommand, CommandResult>>();
            
            cfg.AddBehavior<IPipelineBehavior<RespondToSellOrderCommand, CommandResult>,
                LoggingBehavior<RespondToSellOrderCommand, CommandResult>>();
            
            cfg.AddBehavior<IPipelineBehavior<ConfirmOrderByBuyerCommand, CommandResult>,
                LoggingBehavior<ConfirmOrderByBuyerCommand, CommandResult>>();
            
            cfg.AddBehavior<IPipelineBehavior<ConfirmBySellerAndCompleteOrderCommand, CommandResult>,
                LoggingBehavior<ConfirmBySellerAndCompleteOrderCommand, CommandResult>>();
        });

        return services;
    }
}