using Bogus;
using Core.Application.Services;
using Core.Domain.Enums;
using Infrastructure.Storage.Constants;
using Infrastructure.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;
using Entities = Core.Domain.Entities;
using Models = Infrastructure.Storage.Models;

namespace Infrastructure;

public class P2pDexContext : DbContext
{
    private static async Task<Web3> CreateWeb3(string exchangerAccountAddress, string exchangerAccountPassword, string netUrl)
    {
        var exchangerAccount = new ManagedAccount(exchangerAccountAddress, exchangerAccountPassword);
        var testWeb3 = new Web3(exchangerAccount, netUrl);
        //var unlockedAccounts = await testWeb3.Personal.ListAccounts.SendRequestAsync();
        //if (!Array.Exists(unlockedAccounts, ua => ua == exchangerAccountAddress))
        //    throw new Exception("Failed to unlock exchanger account. Address and/or password are invalid.");

        return testWeb3;
    }
    
    public static async Task EnsureCreatedAndLoadTestData(P2pDexContext dbContext)
    {
        var faker = new Faker("ru");
        var exchangerAccountAddress = "REPLACE_THIS_ADDRESS";
        var web3 = await CreateWeb3(exchangerAccountAddress, "REPLACE_THIS_PASSWORD", "REPLACE_THIS_NET_URL");
        var concurrentWeb3Wrapper = new ConcurrentWeb3Wrapper(web3);
        var blockchain = new EthereumBlockchain(concurrentWeb3Wrapper, exchangerAccountAddress);
        var feeCalculator = new FeeCalculator(blockchain, 0.0005m);
        var traderCount = faker.Random.Int(500, 1000);
        
        var traders = new List<Entities.Trader>(traderCount);
        var orders = new List<Entities.OrderBase>(traderCount * 3);
        
        for (var i = 0; i < traderCount; i++)
            traders.Add(new Entities.Trader(faker.Random.Guid().ToString(), faker.Name.FullName()));

        foreach (var trader in traders)
        {
            for (var i = 0; i < faker.Random.Int(0, 3); i++)
            {
                var cryptoAmount = faker.Random.Decimal(0.002m, 1.5m);
                orders.Add(new Entities.SellOrder(faker.Random.Guid().ToString(), Cryptocurrency.Ethereum, cryptoAmount,
                    FiatCurrency.Ruble, faker.Random.Decimal(305_000m, 319_000m), faker.Random.String(20), trader,
                    await feeCalculator.Calculate(cryptoAmount)));
            }
        }

        await dbContext.Traders.AddRangeAsync(traders.Select(t => new Trader
            { Guid = t.Guid, Name = t.Name, BuyerRating = t.BuyerRating, SellerRating = t.SellerRating }));
        await dbContext.Orders.AddRangeAsync(orders.Select(o => new Order
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

    public P2pDexContext(DbContextOptions<P2pDexContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Trader>()
            .HasMany<Order>()
            .WithOne()
            .HasForeignKey(o => o.BuyerGuid)
            .IsRequired(false);

        modelBuilder.Entity<Trader>()
            .HasMany<Order>()
            .WithOne()
            .HasForeignKey(o => o.SellerGuid)
            .IsRequired(false);
    }

    public DbSet<Trader> Traders { get; set; } = null!;

    public DbSet<Order> Orders { get; set; } = null!;
}