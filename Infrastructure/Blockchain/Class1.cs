using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;

namespace Infrastructure;

public class Class1
{
    public async Task Handle(string exchangerAccountAddress, string exchangerAccountPassword, string netUrl)
    {
        var exchangerAccount = new ManagedAccount(exchangerAccountAddress, exchangerAccountPassword);
        var testWeb3 = new Web3(exchangerAccount, netUrl);
        var unlockedAccounts = await testWeb3.Personal.ListAccounts.SendRequestAsync();
        if (!Array.Exists(unlockedAccounts, ua => ua == exchangerAccountAddress))
            throw new Exception("Failed to unlock exchanger account. Address and/or password are invalid.");
    }
}