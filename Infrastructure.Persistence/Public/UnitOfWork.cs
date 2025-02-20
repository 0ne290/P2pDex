using Core.Application.Private.Interfaces;
using Infrastructure.Persistence.Private;

namespace Infrastructure.Persistence.Public;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(P2PDexDbContext dbContext)
    {
        _dbContext = dbContext;
        
        Repository = new Repository(dbContext);
        SellOrdersAndTheirSellersQuery = new SellOrdersAndTheirSellersQuery(dbContext);
        SellOrderAndItsTradersQuery = new SellOrderAndItsTradersQuery(dbContext);
    }

    public async Task SaveAllTrackedEntities() => await _dbContext.SaveChangesAsync();

    public void UntrackAllEntities() => _dbContext.ChangeTracker.Clear();

    public IRepository Repository { get; }
    
    public ISellOrdersAndTheirSellersQuery SellOrdersAndTheirSellersQuery { get; }
    
    public ISellOrderAndItsTradersQuery SellOrderAndItsTradersQuery { get; }

    private readonly P2PDexDbContext _dbContext;
}