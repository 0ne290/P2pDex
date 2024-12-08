using Core.Domain.Interfaces;

namespace Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(P2PDexContext dbContext, Repository repository)
    {
        _dbContext = dbContext;
        Repository = repository;
    }

    public async Task Save() => await _dbContext.SaveChangesAsync();
    
    public IRepository Repository { get; }

    private readonly P2PDexContext _dbContext;
}