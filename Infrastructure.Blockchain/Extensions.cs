using Core.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace Infrastructure.Blockchain;

public static class Extensions
{
    public static async Task<string> AddBlockchain(this IServiceCollection services, string netUrl,
        string encryptedKeystore, string exchangerAccountPassword, double feeUpdateIntervalInMs)
    {
        //var exchangerAccount = Account.LoadFromKeyStore(encryptedKeystore, exchangerAccountPassword);
        var exchangerAccount = new Account("1b5f0a98baed0089e4d0e2d7b5d1eb58f779d018c3f1ff75b025e447ef72eab0");
        var testWeb3 = new Web3(exchangerAccount, netUrl);
        var unlockedAccounts = await testWeb3.Personal.ListAccounts.SendRequestAsync();
        if (unlockedAccounts.Length != 1)
            throw new Exception("Failed to unlock exchanger account. Address and/or password are invalid.");

        services.AddKeyedSingleton<Web3>("Singleton",
            (_, _) => new Web3(exchangerAccount));
        services.AddKeyedScoped<Web3>("Scoped",
            (_, _) => new Web3(exchangerAccount));

        services.AddSingleton<TransferTransactionFeeTracker>(sp =>
            new TransferTransactionFeeTracker(sp.GetRequiredKeyedService<Web3>("Singleton"), feeUpdateIntervalInMs));

        services.AddScoped<IBlockchain, EthereumBlockchain>(sp =>
            new EthereumBlockchain(sp.GetRequiredKeyedService<Web3>("Scoped"),
                sp.GetRequiredService<TransferTransactionFeeTracker>()));

        return unlockedAccounts[0];
    }
}