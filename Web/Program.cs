using Core.Application;
using Core.Application.BuyOrder.Commands;
using Core.Application.General.Commands;
using Core.Application.SellOrder.Commands;
using Core.Domain.Interfaces;
using Infrastructure.Blockchain;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Formatting.Json;
using Web.ApiKeyAuthScheme;

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

            builder.Services.AddAuthentication().AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthSchemeHandler>("ApiKey",
                opts =>
                {
                    opts.ApiKey =
                        "qxTsEGrZru84hyfnlhBWUHqsJm2p/XUdD417YCvifUcNaOGnhGauARJz3Dq8RAWI1Sj26grwAYAOtLzr9eaidA==";
                });

            await CompositionRoot(builder.Services, builder.Configuration);

            // Add services to the container.
            builder.Services.AddSerilog().AddControllers().AddNewtonsoftJson();

            var app = builder.Build();

            app.UseSerilogRequestLogging();

            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
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

        const double blockchainFeeTrackIntervalInMinutes = 20d;
        var exchangerFeeRateInPercent = decimal.Parse(exchangerConfig["FeeRate"] ??
                                                      throw new Exception("Config.Exchanger.FeeRate is not found."));
        const double orderTransferTransactionTrackIntervalInMinutes = 2d;

        await AddPersistence(persistenceConfig["SqliteConnectionString"] ??
                             throw new Exception("Config.Persistence.SqliteConnectionString is not found."));
        var blockchainAccountAddress = await AddBlockchain(
            blockchainConfig["PrivateKey"] ?? throw new Exception("Config.Blockchain.PrivateKey is not found."),
            int.Parse(blockchainConfig["ChainId"] ?? throw new Exception("Config.Blockchain.ChainId is not found.")),
            blockchainConfig["Url"] ?? throw new Exception("Config.Blockchain.Url is not found."),
            TimeSpan.FromMinutes(blockchainFeeTrackIntervalInMinutes).TotalMilliseconds);
        AddApplication(exchangerFeeRateInPercent / 100, blockchainAccountAddress.ToLower(),
            TimeSpan.FromMinutes(orderTransferTransactionTrackIntervalInMinutes).TotalMilliseconds);

        return;

        async Task AddPersistence(string connectionString)
        {
            Action<DbContextOptionsBuilder> optionsAction = options => options.UseSqlite(connectionString);

            var dbContextOptionsBuilder = new DbContextOptionsBuilder();
            optionsAction(dbContextOptionsBuilder);

            var testDbContext = new P2PDexDbContext(dbContextOptionsBuilder.Options);
            await testDbContext.Database.EnsureCreatedAsync();

            services.AddDbContext<P2PDexDbContext>(optionsAction, ServiceLifetime.Transient, ServiceLifetime.Transient);

            services.AddTransient<Repository>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }

        async Task<string> AddBlockchain(string accountPrivateKey, int id, string url, double feeTrackIntervalInMs)
        {
            var blockchainAccount = new Account(accountPrivateKey, id);
            Func<Web3> web3Factory = () => new Web3(blockchainAccount, url);

            var testWeb3 = web3Factory();
            await testWeb3.Eth.GasPrice.SendRequestAsync();

            services.AddSingleton(_ => web3Factory());

            services.AddSingleton<FeeTracker>(
                sp => new FeeTracker(sp.GetRequiredService<Web3>(), feeTrackIntervalInMs));

            services.AddSingleton<IBlockchain, EthereumBlockchain>(sp =>
                new EthereumBlockchain(sp.GetRequiredService<Web3>(), blockchainAccount.Address,
                    sp.GetRequiredService<FeeTracker>()));

            return blockchainAccount.Address;
        }

        void AddApplication(decimal exchangerFeeRate, string exchangerAccountAddress, double orderTransferTransactionTrackIntervalInMs)
        {
            services.AddSingleton(_ => new ExchangerConfiguration(exchangerFeeRate, exchangerAccountAddress));

            services.AddSingleton<OrderTransferTransactionTracker>(sp =>
                new OrderTransferTransactionTracker(sp.GetRequiredService<IBlockchain>(),
                    sp.GetRequiredService<IUnitOfWork>(), sp.GetRequiredService<ExchangerConfiguration>(),
                    sp.GetRequiredService<ILogger<OrderTransferTransactionTracker>>(),
                    orderTransferTransactionTrackIntervalInMs));

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateSellOrderCommand).Assembly);
                
                cfg.AddBehavior<IPipelineBehavior<CalculateFinalCryptoAmountForTransferCommand, CommandResult>,
                    LoggingBehavior<CalculateFinalCryptoAmountForTransferCommand, CommandResult>>();

                cfg.AddBehavior<IPipelineBehavior<GetExchangerAccountAddressCommand, CommandResult>,
                    LoggingBehavior<GetExchangerAccountAddressCommand, CommandResult>>();

                cfg.AddBehavior<IPipelineBehavior<CreateTraderCommand, CommandResult>,
                    LoggingBehavior<CreateTraderCommand, CommandResult>>();

                cfg.AddBehavior<IPipelineBehavior<CreateSellOrderCommand, CommandResult>,
                    LoggingBehavior<CreateSellOrderCommand, CommandResult>>();

                cfg.AddBehavior<IPipelineBehavior<RespondToSellOrderByBuyerCommand, CommandResult>,
                    LoggingBehavior<RespondToSellOrderByBuyerCommand, CommandResult>>();

                cfg.AddBehavior<IPipelineBehavior<ConfirmTransferFiatToSellerByBuyerForSellOrderCommand, CommandResult>,
                    LoggingBehavior<ConfirmTransferFiatToSellerByBuyerForSellOrderCommand, CommandResult>>();

                cfg.AddBehavior<IPipelineBehavior<ConfirmReceiptFiatFromBuyerBySellerForSellOrderCommand, CommandResult>
                    , LoggingBehavior<ConfirmReceiptFiatFromBuyerBySellerForSellOrderCommand, CommandResult>>();
                
                cfg.AddBehavior<IPipelineBehavior<CreateBuyOrderCommand, CommandResult>,
                    LoggingBehavior<CreateBuyOrderCommand, CommandResult>>();

                cfg.AddBehavior<IPipelineBehavior<RespondToBuyOrderBySellerCommand, CommandResult>,
                    LoggingBehavior<RespondToBuyOrderBySellerCommand, CommandResult>>();

                cfg.AddBehavior<IPipelineBehavior<ConfirmTransferFiatToSellerByBuyerForBuyOrderCommand, CommandResult>,
                    LoggingBehavior<ConfirmTransferFiatToSellerByBuyerForBuyOrderCommand, CommandResult>>();

                cfg.AddBehavior<IPipelineBehavior<ConfirmReceiptFiatFromBuyerBySellerForBuyOrderCommand, CommandResult>
                    , LoggingBehavior<ConfirmReceiptFiatFromBuyerBySellerForBuyOrderCommand, CommandResult>>();
            });

            // Вот таким образом можно тонко настраивать создание обработчиков команд
            //services.AddScoped<IRequestHandler<CreateSellOrderCommand, CommandResult>, CreateSellOrderHandler>(sp =>
            //    new CreateSellOrderHandler(sp.GetRequiredService<IBlockchain>(),
            //        sp.GetRequiredKeyedService<IUnitOfWork>("Scoped"), sp.GetRequiredService<ExchangerConfiguration>(),
            //        sp.GetRequiredService<OrderTransferTransactionTracker>()));
        }
    }
}