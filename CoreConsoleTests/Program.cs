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
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Async(c => c.Console())
            .CreateLogger();
        
        var services = new ServiceCollection();
        
        services.AddSingleton<OrderTracker>(sp => new OrderTracker(Array.Empty<Order>(), new BlockchainStub(Array.Empty<(string, bool)>()), new OrderStorageStub(Array.Empty<Order>())));

        var serviceProvider = services.BuildServiceProvider();

        var tracker = serviceProvider.GetRequiredService<OrderTracker>();
        tracker.TrackOrder(Order.CreateSellOrder("2yff", new Trader("dgft,", "yhh", 0, 0), "533ugccc", Cryptocurrency.Ethereum, 0.06m, FiatCurrency.Ruble, 313_486.74m, "Картой МИР епта"));
        Thread.Sleep(15000);
        tracker.Dispose();
    }
}