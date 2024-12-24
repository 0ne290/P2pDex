using Core.Domain.Interfaces;

namespace Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(P2PDexDbContext dbContext, Repository repository)
    {
        _dbContext = dbContext;
        Repository = repository;
    }

    public async Task SaveAllTrackedEntities() => await _dbContext.SaveChangesAsync();
    
    public IRepository Repository { get; }

    private readonly P2PDexDbContext _dbContext;
}