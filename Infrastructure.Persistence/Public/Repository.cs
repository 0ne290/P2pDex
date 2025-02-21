using System.Linq.Expressions;
using Core.Application.Private.Interfaces;
using Infrastructure.Persistence.Private;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Public;

public class Repository : IRepository
{
    public Repository(P2PDexDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add<TEntity>(TEntity entity) where TEntity : class =>
        await _dbContext.Set<TEntity>().AddAsync(entity);

    public void UpdateRange<TEntity>(IEnumerable<TEntity> updatedEntities) where TEntity : class =>
        _dbContext.Set<TEntity>().UpdateRange(updatedEntities);

    public async Task<bool> Exists<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class =>
        await _dbContext.Set<TEntity>().AnyAsync(filter);

    public async Task<TEntity?> TryGet<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class =>
        await _dbContext.Set<TEntity>().FirstOrDefaultAsync(filter);

    public async Task<ICollection<TEntity>> GetAllBy<TEntity>(Expression<Func<TEntity, bool>> filter)
        where TEntity : class => await _dbContext.Set<TEntity>().AsNoTracking().Where(filter).ToListAsync();

    private readonly P2PDexDbContext _dbContext;
}