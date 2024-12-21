using Core.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.Web3;

namespace Infrastructure.Blockchain;

public static class Extensions
{
    public static async Task<IServiceCollection> AddBlockchain(this IServiceCollection services, Func<IServiceProvider, Web3> web3Factory, double feeUpdateIntervalInMs, string accountAddress)
    {
        var testWeb3 = web3Factory(services.BuildServiceProvider());
        await testWeb3.Eth.GasPrice.SendRequestAsync();

        services.AddSingleton(web3Factory);
        
        services.AddSingleton<FeeTracker>(sp => new FeeTracker(sp.GetRequiredService<Web3>(), feeUpdateIntervalInMs));

        services.AddSingleton<IBlockchain, EthereumBlockchain>(sp =>
            new EthereumBlockchain(sp.GetRequiredService<Web3>(), accountAddress, sp.GetRequiredService<FeeTracker>()));

        return services;
    }
}