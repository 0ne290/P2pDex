using Core.Application;
using Infrastructure.Blockchain;
using Infrastructure.Persistence;
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
            builder.Services.AddControllers().AddNewtonsoftJson();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

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
        var persistenceConfig = config.GetRequiredSection("Persistence");

        var netUrl = blockchainConfig["NetUrl"] ?? throw new Exception("Config.Blockchain.NetUrl is not found.");
        var exchangerWalletAddress = blockchainConfig["ExchangerWalletAddress"] ?? throw new Exception("Config.Blockchain.ExchangerWalletAddress is not found.");
        var exchangerWalletPassword = blockchainConfig["ExchangerWalletPassword"] ?? throw new Exception("Config.Blockchain.ExchangerWalletPassword is not found.");
        var exchangerFeeRateInPercent = decimal.Parse(blockchainConfig["ExchangerFeeRate"] ?? throw new Exception("Config.Blockchain.ExchangerFeeRate is not found."));
        var exchangerFeeRate = exchangerFeeRateInPercent / 100;
        const double transferTransactionFeeUpdateIntervalInMinutes = 20d;
        
        var connectionString = persistenceConfig["ConnectionString"] ?? throw new Exception("Config.Persistence.ConnectionString is not found.");

        await services.AddPersistence(connectionString);
        await services.AddBlockchain(netUrl, exchangerWalletAddress, exchangerWalletPassword,
            TimeSpan.FromMinutes(transferTransactionFeeUpdateIntervalInMinutes).TotalMilliseconds);
        services.AddApplication(exchangerWalletAddress, exchangerFeeRate);
    }
}