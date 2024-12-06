﻿using Serilog;
using Serilog.Formatting.Json;

namespace Tests;

internal class Program
{
    private static void Main()
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Async(c => c.Console(new JsonFormatter())).CreateLogger();
        Dictionary<string, int> m = new([new KeyValuePair<string, int>("fnh", 44)]);
        Log.Information("Zshv {@M}.", m);
        Log.CloseAndFlush();
    }

    //private static void TransactionTrackerTest1()
    //{
    //    var faker = new Faker("ru");
//
    //    var transactions = new[]
    //    {
    //        faker.Random.Hash(), faker.Random.Hash(), faker.Random.Hash(), faker.Random.Hash(), faker.Random.Hash(),
    //        faker.Random.Hash(), faker.Random.Hash(), faker.Random.Hash(), faker.Random.Hash()
    //    };
    //    var trader = new Trader(faker.Random.Guid().ToString(), faker.Name.FullName());
    //    var orders = new[]
    //    {
    //        BuyOrder.CreateSellOrder(faker.Random.Guid().ToString(), trader, transactions[0], Cryptocurrency.Ethereum,
    //            faker.Random.Decimal(0.05m, 0.3m),
    //            FiatCurrency.Ruble, faker.Random.Decimal(305_000m, 319_000m), faker.Random.String(20)),
    //        BuyOrder.CreateSellOrder(faker.Random.Guid().ToString(), trader, transactions[1], Cryptocurrency.Ethereum,
    //            faker.Random.Decimal(0.05m, 0.3m),
    //            FiatCurrency.Ruble, faker.Random.Decimal(305_000m, 319_000m), faker.Random.String(20)),
    //        BuyOrder.CreateSellOrder(faker.Random.Guid().ToString(), trader, transactions[2], Cryptocurrency.Ethereum,
    //            faker.Random.Decimal(0.05m, 0.3m),
    //            FiatCurrency.Ruble, faker.Random.Decimal(305_000m, 319_000m), faker.Random.String(20)),
    //        BuyOrder.CreateSellOrder(faker.Random.Guid().ToString(), trader, transactions[3], Cryptocurrency.Ethereum,
    //            faker.Random.Decimal(0.05m, 0.3m),
    //            FiatCurrency.Ruble, faker.Random.Decimal(305_000m, 319_000m), faker.Random.String(20)),
    //        BuyOrder.CreateSellOrder(faker.Random.Guid().ToString(), trader, transactions[4], Cryptocurrency.Ethereum,
    //            faker.Random.Decimal(0.05m, 0.3m),
    //            FiatCurrency.Ruble, faker.Random.Decimal(305_000m, 319_000m), faker.Random.String(20)),
    //        BuyOrder.CreateSellOrder(faker.Random.Guid().ToString(), trader, transactions[5], Cryptocurrency.Ethereum,
    //            faker.Random.Decimal(0.05m, 0.3m),
    //            FiatCurrency.Ruble, faker.Random.Decimal(305_000m, 319_000m), faker.Random.String(20)),
    //        BuyOrder.CreateSellOrder(faker.Random.Guid().ToString(), trader, transactions[6], Cryptocurrency.Ethereum,
    //            faker.Random.Decimal(0.05m, 0.3m),
    //            FiatCurrency.Ruble, faker.Random.Decimal(305_000m, 319_000m), faker.Random.String(20)),
    //        BuyOrder.CreateSellOrder(faker.Random.Guid().ToString(), trader, transactions[7], Cryptocurrency.Ethereum,
    //            faker.Random.Decimal(0.05m, 0.3m),
    //            FiatCurrency.Ruble, faker.Random.Decimal(305_000m, 319_000m), faker.Random.String(20)),
    //        BuyOrder.CreateSellOrder(faker.Random.Guid().ToString(), trader, transactions[8], Cryptocurrency.Ethereum,
    //            faker.Random.Decimal(0.05m, 0.3m),
    //            FiatCurrency.Ruble, faker.Random.Decimal(305_000m, 319_000m), faker.Random.String(20)),
    //    };
//
    //    Log.Logger = new LoggerConfiguration()
    //        .WriteTo.Async(c => c.Console())
    //        .CreateLogger();
//
    //    var services = new ServiceCollection();
    //    services.AddSingleton<IOrderStorage, OrderStorageStub>(_ => new OrderStorageStub(orders));
    //    services.AddSingleton<IBlockchain, BlockchainStub>(
    //        _ => new BlockchainStub(transactions.Select(t => (t, false))));
    //    services.AddSingleton<OrderTracker>(sp =>
    //        new OrderTracker(orders, sp.GetRequiredService<IBlockchain>(), sp.GetRequiredService<IOrderStorage>()));
//
    //    var serviceProvider = services.BuildServiceProvider();
    //    var tracker = serviceProvider.GetRequiredService<OrderTracker>();
    //    var blockchain = (BlockchainStub)serviceProvider.GetRequiredService<IBlockchain>();
    //    var orderStorage = (OrderStorageStub)serviceProvider.GetRequiredService<IOrderStorage>();
//
    //    while (true)
    //    {
    //        var input = Console.ReadLine();
    //        if (input == "c")
    //            blockchain.ConfirmTransaction(int.Parse(Console.ReadLine()!));
    //        else if (input == "a&t")
    //        {
    //            var transaction = faker.Random.Hash();
    //            var sellOrder = BuyOrder.CreateSellOrder(faker.Random.Guid().ToString(), trader, transaction,
    //                Cryptocurrency.Ethereum, faker.Random.Decimal(0.05m, 0.3m),
    //                FiatCurrency.Ruble, faker.Random.Decimal(305_000m, 319_000m), faker.Random.String(20));
    //            blockchain.AddTransaction(transaction);
    //            orderStorage.Add(sellOrder);
    //            tracker.TrackOrder(sellOrder);
    //        }
    //        else
    //            break;
    //    }
//
    //    foreach (var sellOrder in orderStorage.GetAll())
    //        Log.Logger.Information("BuyOrder {@BuyOrder}.",
    //            new
    //            {
    //                sellOrder.Guid, sellOrder.Type, sellOrder.CurrentStatus, sellOrder.TransactionHash
    //            });
//
    //    serviceProvider.Dispose();
//
    //    Log.CloseAndFlush();
    //}
//
    //private static void TransactionTrackerTest2()
    //{
    //    var faker = new Faker("ru");
//
    //    var transactions = new[]
    //    {
    //        "0x0f24287c882effb869166e1d0e0d3716ec012ac64b547c2abb852280e694e4ce"
    //    };
    //    var trader = new Trader(faker.Random.Guid().ToString(), faker.Name.FullName());
    //    var orders = new[]
    //    {
    //        BuyOrder.CreateSellOrder(faker.Random.Guid().ToString(), trader, transactions[0], Cryptocurrency.Ethereum,
    //            faker.Random.Decimal(0.05m, 0.3m),
    //            FiatCurrency.Ruble, faker.Random.Decimal(305_000m, 319_000m), faker.Random.String(20))
    //    };
//
    //    Log.Logger = new LoggerConfiguration()
    //        .WriteTo.Async(c => c.Console())
    //        .CreateLogger();
//
    //    var services = new ServiceCollection();
    //    services.AddSingleton<IOrderStorage, OrderStorageStub>(_ => new OrderStorageStub(orders));
    //    services.AddSingleton<Web3>(_ => new Web3("http://192.168.0.102:8545"));
    //    services.AddSingleton<IBlockchain, EthereumBlockchain>();
    //    services.AddSingleton<OrderTracker>(sp =>
    //        new OrderTracker(orders, sp.GetRequiredService<IBlockchain>(), sp.GetRequiredService<IOrderStorage>()));
//
    //    var serviceProvider = services.BuildServiceProvider();
    //    var tracker = serviceProvider.GetRequiredService<OrderTracker>();
    //    var orderStorage = (OrderStorageStub)serviceProvider.GetRequiredService<IOrderStorage>();
//
    //    while (true)
    //    {
    //        var input = Console.ReadLine();
    //        if (input == "a&t")
    //        {
    //            var transaction = Console.ReadLine()!;
    //            var sellOrder = BuyOrder.CreateSellOrder(faker.Random.Guid().ToString(), trader, transaction,
    //                Cryptocurrency.Ethereum, faker.Random.Decimal(0.05m, 0.3m),
    //                FiatCurrency.Ruble, faker.Random.Decimal(305_000m, 319_000m), faker.Random.String(20));
    //            orderStorage.Add(sellOrder);
    //            tracker.TrackOrder(sellOrder);
    //        }
    //        else
    //            break;
    //    }
//
    //    foreach (var sellOrder in orderStorage.GetAll())
    //        Log.Logger.Information("BuyOrder {@BuyOrder}.",
    //            new
    //            {
    //                sellOrder.Guid, sellOrder.Type, sellOrder.CurrentStatus, sellOrder.TransactionHash
    //            });
//
    //    serviceProvider.Dispose();
//
    //    Log.CloseAndFlush();
    //}
}