using Core.Application;
using Infrastructure.Blockchain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Formatting.Json;

namespace Web;

public class Program
{
    public static async Task Main()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Configuration.GetRequiredSection("Kestrel");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)

            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers([new DbUpdateExceptionDestructurer()]))

            .WriteTo.Async(a => a.File(new JsonFormatter(),
                builder.Configuration.GetRequiredSection("CustomLogging")["FilePath"] ??
                throw new Exception("Config.CustomLogging.FilePath is not found"), retainedFileCountLimit: 4,
                rollOnFileSizeLimit: true, fileSizeLimitBytes: 5_368_709_120))

            .CreateLogger();

        try
        {
            Log.Information("Starting host build.");

            await CompositionRoot(builder.Services, builder.Configuration);

            // Add services to the container.
            builder.Services.AddSerilog().AddControllers().AddNewtonsoftJson();

            var app = builder.Build();

            app.UseSerilogRequestLogging();

            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Api}/{action=Index}/{id?}");

            Log.Information("Success to build host. Starting web application.");

            await app.RunAsync();
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Failed to build host.");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static async Task CompositionRoot(IServiceCollection services, IConfiguration config)
    {
        var blockchainConfig = config.GetRequiredSection("Blockchain");
        var exchangerConfig = config.GetRequiredSection("Exchanger");
        var persistenceConfig = config.GetRequiredSection("Persistence");

        var blockchainUrl = blockchainConfig["Url"] ?? throw new Exception("Config.Blockchain.Url is not found.");
        var blockchainAccountPrivateKey = blockchainConfig["PrivateKey"] ??
                                          throw new Exception("Config.Blockchain.PrivateKey is not found.");
        var blockchainId = int.Parse(blockchainConfig["ChainId"] ??
                                     throw new Exception("Config.Blockchain.ChainId is not found."));
        var blockchainAccount = new Account(blockchainAccountPrivateKey, blockchainId);
        const double transferTransactionFeeUpdateIntervalInMinutes = 20d;

        var exchangerFeeRateInPercent = decimal.Parse(exchangerConfig["FeeRate"] ??
                                                      throw new Exception("Config.Exchanger.FeeRate is not found."));
        var exchangerFeeRate = exchangerFeeRateInPercent / 100;

        var connectionString = persistenceConfig["SqliteConnectionString"] ??
                               throw new Exception("Config.Persistence.SqliteConnectionString is not found.");

        await services.AddPersistence(options => options.UseSqlite(connectionString));
        await services.AddBlockchain(_ => new Web3(blockchainAccount, blockchainUrl),
            TimeSpan.FromMinutes(transferTransactionFeeUpdateIntervalInMinutes).TotalMilliseconds, blockchainAccount.Address);
        services.AddApplication(_ => new ExchangerConfiguration(exchangerFeeRate, blockchainAccount.Address.ToLower()));
    }
}