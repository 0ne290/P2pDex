using Core.Application.Private.Interfaces;
using Infrastructure.Persistence.Private;

namespace Infrastructure.Persistence.Public;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(Repository repository)
    {
        _dbContext = repository.DbContext;
        Repository = repository;
    }

    public async Task SaveAllTrackedEntities() => await _dbContext.SaveChangesAsync();

    public void UntrackAllEntities() => _dbContext.ChangeTracker.Clear();

    public IRepository Repository { get; }

    private readonly P2PDexDbContext _dbContext;
}