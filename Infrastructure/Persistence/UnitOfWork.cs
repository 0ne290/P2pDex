using Core.Domain.Interfaces;

namespace Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(P2pDexContext dbContext, Repository repository)
    {
        _dbContext = dbContext;
        Repository = repository;
    }

    public async Task Save() => await _dbContext.SaveChangesAsync();
    
    public IRepository Repository { get; }

    private readonly P2pDexContext _dbContext;
}