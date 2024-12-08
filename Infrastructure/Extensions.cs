using Infrastructure.Blockchain.Services;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;

namespace Infrastructure;

public static class Extensions
{
    public static async Task<IServiceCollection> AddBlockchain(this IServiceCollection services, string netUrl, string exchangerWalletAddress, string walletPassword, double feeUpdateIntervalInMs)
    {
        var exchangerAccount = new ManagedAccount(exchangerWalletAddress, walletPassword);
        var testWeb3 = new Web3(exchangerAccount, netUrl);
        var unlockedAccounts = await testWeb3.Personal.ListAccounts.SendRequestAsync();
        if (!Array.Exists(unlockedAccounts, ua => ua == exchangerAccountAddress))
            throw new Exception("Failed to unlock exchanger account. Address and/or password are invalid.");
        
        services.AddKeyedSingleton<Web3>("Singleton", () => new Web3(new ManagedAccount(exchangerWalletAddress, walletPassword)));
        services.AddKeyedScoped<Web3>("Scoped", () => new Web3(new ManagedAccount(exchangerWalletAddress, walletPassword)));
        
        services.AddSingleton<TransferTransactionFeeTracker>(sp =>
            new TransferTransactionFeeTracker(sp.GetRequiredKeyedService<Web3>("Singleton"), feeUpdateIntervalInMs));

        services.AddScoped<IBlockchain, EthereumBlockchain>(sp => new EthereumBlockchain(sp.GetRequiredKeyedService<Web3>("Scoped"), sp.GetRequiredService<TransferTransactionFeeTracker>()));
        
        return services;
    }

    public static async Task<IServiceCollection> AddPersistence(this IServiceCollection services, string connectionString)
    {
        var testDbContext = new P2PDbContext((new DbContextOptionsBuilder()).UseSqlite(connectionString).Options);
        await testDbContext.Database.EnsureCreatedAsync();
        
        services.AddDbContext<P2PDbContext>(options => options.UseSqlite(connectionString));

        services.AddScoped(IRepository, Repository);
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services
    }
}
