using Core.Application.Private.Interfaces;
using Infrastructure.Persistence.Private;

namespace Infrastructure.Persistence.Public;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(P2PDexDbContext dbContext)
    {
        _dbContext = dbContext;
        
        Repository = new Repository(dbContext);
        SellOrdersWithSellersQuery = new SellOrdersWithSellersQuery(dbContext);
        SellOrderWithTradersQuery = new SellOrderWithTradersQuery(dbContext);
    }

    public async Task SaveAllTrackedEntities() => await _dbContext.SaveChangesAsync();

    public void UntrackAllEntities() => _dbContext.ChangeTracker.Clear();

    public IRepository Repository { get; }
    
    public ISellOrdersWithSellersQuery SellOrdersWithSellersQuery { get; }
    
    public ISellOrderWithTradersQuery SellOrderWithTradersQuery { get; }

    private readonly P2PDexDbContext _dbContext;
}