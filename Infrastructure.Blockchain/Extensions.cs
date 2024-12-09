using Core.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;

namespace Infrastructure.Blockchain;

public static class Extensions
{
    public static async Task<IServiceCollection> AddBlockchain(this IServiceCollection services, string netUrl,
        string accountAddress, string accountPassword, double feeUpdateIntervalInMs)
    {
        var exchangerAccount = new ManagedAccount(accountAddress, accountPassword);
        var testWeb3 = new Web3(exchangerAccount, netUrl);
        var unlockedAccounts = await testWeb3.Personal.ListAccounts.SendRequestAsync();
        if (!Array.Exists(unlockedAccounts, ua => ua == accountAddress))
            throw new Exception("Failed to unlock exchanger account. Address and/or password are invalid.");

        services.AddKeyedSingleton<Web3>("Singleton",
            (_, _) => new Web3(new ManagedAccount(accountAddress, accountPassword)));
        services.AddKeyedScoped<Web3>("Scoped",
            (_, _) => new Web3(new ManagedAccount(accountAddress, accountPassword)));

        services.AddSingleton<TransferTransactionFeeTracker>(sp =>
            new TransferTransactionFeeTracker(sp.GetRequiredKeyedService<Web3>("Singleton"), feeUpdateIntervalInMs));

        services.AddScoped<IBlockchain, EthereumBlockchain>(sp =>
            new EthereumBlockchain(sp.GetRequiredKeyedService<Web3>("Scoped"),
                sp.GetRequiredService<TransferTransactionFeeTracker>()));

        return services;
    }
}
