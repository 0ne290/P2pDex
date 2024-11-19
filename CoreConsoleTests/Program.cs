using Core.Enums;
using Core.Interfaces;
using Core.Models;
using CoreConsoleTests.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace CoreConsoleTests;

class Program
{
    static void Main(string[] args)
    {
        var transactions = [ "us656", "if753r", "DYHHe5e", "088cFstDSyhR3", "+$4365&7yfyg-$-arwYyi", "xyr5", "656", "*&36", "rrtFyDDR" ];
            var trader = new Trader("dgft,", "yhh", 0, 0);
            var orders = new Order[]
            {
                Order.CreateSellOrder("dyr66yfii6t", trader, transactions[0], Cryptocurrency.Ethereum, 0.06m, FiatCurrency.Ruble, 313_486.74m, "Картой МИР епта"),
                Order.CreateSellOrder("7tyifyry57", trader, transactions[1], Cryptocurrency.Ethereum, 0.06m, FiatCurrency.Ruble, 313_486.74m, "Картой МИР епта"),
                Order.CreateSellOrder("96836wtcbjt", trader, transactions[2], Cryptocurrency.Ethereum, 0.06m, FiatCurrency.Ruble, 313_486.74m, "Картой МИР епта"),
                Order.CreateSellOrder("_*_77jfut", trader, transactions[3], Cryptocurrency.Ethereum, 0.06m, FiatCurrency.Ruble, 313_486.74m, "Картой МИР епта"),
                Order.CreateSellOrder("iyyi6957r", trader, transactions[4], Cryptocurrency.Ethereum, 0.06m, FiatCurrency.Ruble, 313_486.74m, "Картой МИР епта"),
                Order.CreateSellOrder("cufr8e66Yy7", trader, transactions[5], Cryptocurrency.Ethereum, 0.06m, FiatCurrency.Ruble, 313_486.74m, "Картой МИР епта"),
                Order.CreateSellOrder("0847_@2luu", trader, transactions[6], Cryptocurrency.Ethereum, 0.06m, FiatCurrency.Ruble, 313_486.74m, "Картой МИР епта"),
                Order.CreateSellOrder("ksouy7r664ruit", trader, transactions[7], Cryptocurrency.Ethereum, 0.06m, FiatCurrency.Ruble, 313_486.74m, "Картой МИР епта"),
                Order.CreateSellOrder("da37ofog46ru", trader, transactions[8], Cryptocurrency.Ethereum, 0.06m, FiatCurrency.Ruble, 313_486.74m, "Картой МИР епта"),
            };
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Async(c => c.Console())
            .CreateLogger();
        
        var services = new ServiceCollection();
        services.AddSingleton<IOrderStorage, OrderStorageStub>(_ => new OrderStorageStub(orders));
        services.AddSingleton<IBlockchain, BlockchainStub>(_ => new BlockchainStub(transactions.Select(t => (t, false))));
        services.AddSingleton<OrderTracker>(sp => new OrderTracker(orders, sp.GetRequiredService<IBlockchain>(), sp.GetRequiredService<IOrderService>()));

        var serviceProvider = services.BuildServiceProvider();
        var tracker = serviceProvider.GetRequiredService<OrderTracker>();
        var blockchain = sp.GetRequiredService<IBlockchain>();

        while (Console.ReadLine() == "confirm")
            blockchain.Confirm(int.Parse(Console.ReadLine()));
        
        serviceProvider.Dispose();
    }
}
