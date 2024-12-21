using Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class Extensions
{
    public static async Task<IServiceCollection> AddPersistence(this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction)
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder();
        optionsAction(dbContextOptionsBuilder);
        
        var testDbContext =
            new P2PDexDbContext(dbContextOptionsBuilder.Options);
        await testDbContext.Database.EnsureCreatedAsync();

        services.AddDbContext<P2PDexDbContext>(optionsAction);

        services.AddScoped<IRepository, Repository>();
        services.AddScoped<Repository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
