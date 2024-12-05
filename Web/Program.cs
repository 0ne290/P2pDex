using Core.Application.Commands;
using Core.Application.Interfaces;
using Core.Application.PipelineBehaviors;
using Core.Application.Services;
using FluentResults;
using Infrastructure.Blockchain;
using MediatR;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;

namespace Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Api}/{action=Index}/{id?}");

        app.Run();
    }

    private static async Task BuildApplicatioc(IServiceCollection services, string exchangerAccountAddress, string exchangerAccountPassword, string netUrl, double intervalInMs)
    {
        await ValidateWeb3(CreateWeb3(exchangerAccountAddress, exchangerAccountPassword, netUrl), exchangerAccountAddress);
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ConfirmBySellerOfCryptocurrencyTransferTransactionCommand)
                .Assembly);
            cfg.AddBehavior<IPipelineBehavior<IRequest<IResultBase>, IResultBase>, LoggingBehavior>();
        });

        services.AddKeyedSingleton<Web3>("Singleton",
            (_, _) => new Web3(new ManagedAccount(exchangerAccountAddress, exchangerAccountPassword), netUrl));
        services.AddKeyedScoped<Web3>("Scoped",
            (_, _) => new Web3(new ManagedAccount(exchangerAccountAddress, exchangerAccountPassword), netUrl));
        
        services.AddKeyedSingleton<IBlockchain, EthereumBlockchain>("Singleton",
            (serviceProvider, _) => new EthereumBlockchain(serviceProvider.GetRequiredKeyedService<Web3>("Singleton"), exchangerAccountAddress));
        services.AddKeyedScoped<IBlockchain, EthereumBlockchain>("Scoped",
            (serviceProvider, _) => new EthereumBlockchain(serviceProvider.GetRequiredKeyedService<Web3>("Scoped"), exchangerAccountAddress));
        
        

        services.AddSingleton<OrderTransferTransactionTracker>(sp =>
            new OrderTransferTransactionTracker(sp.GetRequiredKeyedService<IBlockchain>("Singleton"), intervalInMs));
    }

    private static Web3 CreateWeb3(string exchangerAccountAddress, string exchangerAccountPassword, string netUrl)
    {
        var exchangerAccount = new ManagedAccount(exchangerAccountAddress, exchangerAccountPassword);
        
        return new Web3(exchangerAccount, netUrl);
    }
    
    private static async Task ValidateWeb3(Web3 web3, string exchangerAccountAddress)
    {
        var unlockedAccounts = await web3.Personal.ListAccounts.SendRequestAsync();
        if (!Array.Exists(unlockedAccounts, ua => ua == exchangerAccountAddress))
            throw new Exception("Failed to unlock exchanger account. Address and/or password are invalid.");
    }
}