using Core.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace Infrastructure.Blockchain;

public static class Extensions
{
    public static async Task<IServiceCollection> AddBlockchain(this IServiceCollection services, string url, Account account, double feeUpdateIntervalInMs)
    {
        var testWeb3 = new Web3(account, url);
        await testWeb3.Eth.GasPrice.SendRequestAsync();

        services.AddKeyedSingleton<Web3>("Singleton",
            (_, _) => new Web3(account, url));
        services.AddKeyedScoped<Web3>("Scoped",
            (_, _) => new Web3(account, url));

        services.AddSingleton<FeePerGasTracker>(sp =>
            new FeePerGasTracker(sp.GetRequiredKeyedService<Web3>("Singleton"), feeUpdateIntervalInMs));

        services.AddScoped<IBlockchain, EthereumBlockchain>(sp =>
            new EthereumBlockchain(sp.GetRequiredKeyedService<Web3>("Scoped"),
                sp.GetRequiredService<FeePerGasTracker>()));

        return services;
    }
}