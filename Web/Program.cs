using Bogus;
using Core.Application.Commands;
using Core.Application.PipelineBehaviors;
using Core.Application.Services;
using Core.Domain.Enums;
using Core.Domain.Interfaces;
using FluentResults;
using Infrastructure.Blockchain;
using Infrastructure.Storage;
using Infrastructure.Storage.Enums;
using MediatR;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;
using DomainEntities = Core.Domain.Entities;
using InfrastructureModels = Infrastructure.Storage.Models;

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
        await CreateTestWeb3(exchangerAccountAddress, exchangerAccountPassword, netUrl);
        
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
    
    private static async Task LoadTestData()
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
        
        var faker = new Faker("ru");
        var exchangerAccountAddress = "REPLACE_THIS_ADDRESS";
        var web3 = await CreateTestWeb3(exchangerAccountAddress, "REPLACE_THIS_PASSWORD", "REPLACE_THIS_NET_URL");
        var blockchain = new EthereumBlockchain(web3, exchangerAccountAddress);
        var feeCalculator = new FeeCalculator(blockchain, 0.0005m);
        var traderCount = faker.Random.Int(500, 1000);
        
        var traders = new List<DomainEntities.Trader>(traderCount);
        var orders = new List<DomainEntities.OrderBase>(traderCount * 3);
        
        for (var i = 0; i < traderCount; i++)
            traders.Add(new DomainEntities.Trader(faker.Random.Guid().ToString(), faker.Name.FullName()));

        foreach (var trader in traders)
        {
            for (var i = 0; i < faker.Random.Int(0, 3); i++)
            {
                var cryptoAmount = faker.Random.Decimal(0.002m, 1.5m);
                orders.Add(new DomainEntities.SellOrder(faker.Random.Guid().ToString(), Cryptocurrency.Ethereum, cryptoAmount,
                    FiatCurrency.Ruble, faker.Random.Decimal(305_000m, 319_000m), faker.Random.String(20), trader,
                    await feeCalculator.Calculate(cryptoAmount)));
            }
        }

        await dbContext.Traders.AddRangeAsync(traders.Select(t => new InfrastructureModels.Trader
            { Guid = t.Guid, Name = t.Name, BuyerRating = t.BuyerRating, SellerRating = t.SellerRating }));
        await dbContext.Orders.AddRangeAsync(orders.Select(o => new InfrastructureModels.Order
        {
            Guid = o.Guid,
            Type = OrderType.Sell,
            Status = o.Status,
            Crypto = o.Crypto,
            CryptoAmount = o.CryptoAmount,
            Fiat = o.Fiat,
            CryptoToFiatExchangeRate = o.CryptoToFiatExchangeRate,
            FiatAmount = o.FiatAmount,
            PaymentMethodInfo = o.PaymentMethodInfo,
            SellerGuid = o.SellerGuid,
            SellerToExchangerFee = o.Fee.SellerToExchanger,
            ExchangerToMinersFee = o.Fee.ExpectedExchangerToMiners,
            TransferTransactionHash = o.TransferTransactionHash,
            BuyerGuid = o.BuyerGuid,
            BuyersWalletAddress = o.BuyersWalletAddress,
        }));

        await dbContext.SaveChangesAsync();
    }
    
    private static async Task<Web3> CreateTestWeb3(string exchangerAccountAddress, string exchangerAccountPassword, string netUrl)
    {
        var exchangerAccount = new ManagedAccount(exchangerAccountAddress, exchangerAccountPassword);
        var testWeb3 = new Web3(exchangerAccount, netUrl);
        var unlockedAccounts = await testWeb3.Personal.ListAccounts.SendRequestAsync();
        if (!Array.Exists(unlockedAccounts, ua => ua == exchangerAccountAddress))
            throw new Exception("Failed to unlock exchanger account. Address and/or password are invalid.");

        return testWeb3;
    }
}