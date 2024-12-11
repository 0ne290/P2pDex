using Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class Extensions
{
    public static async Task<IServiceCollection> AddPersistence(this IServiceCollection services,
        string connectionString)
    {
        var testDbContext =
            new P2PDexDbContext(new DbContextOptionsBuilder<P2PDexDbContext>().UseSqlite(connectionString).Options);
        await testDbContext.Database.EnsureCreatedAsync();

        services.AddDbContext<P2PDexDbContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<IRepository, Repository>();
        services.AddScoped<Repository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
