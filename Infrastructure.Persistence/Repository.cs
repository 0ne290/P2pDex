using System.Linq.Expressions;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class Repository : IRepository
{
    public Repository(P2PDexDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task Add<TEntity>(TEntity entity) where TEntity : BaseEntity =>
        await DbContext.Set<TEntity>().AddAsync(entity);

    public void UpdateRange<TEntity>(IEnumerable<TEntity> updatedEntities) where TEntity : BaseEntity =>
        DbContext.Set<TEntity>().UpdateRange(updatedEntities);

    public async Task<bool> Exists<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity =>
        await DbContext.Set<TEntity>().AnyAsync(filter);

    public async Task<TEntity?> TryGetByGuid<TEntity>(Guid guid) where TEntity : BaseEntity =>
        await DbContext.Set<TEntity>().FirstOrDefaultAsync(e => e.Guid.Equals(guid));

    public async Task<ICollection<TEntity>> GetAll<TEntity>(Expression<Func<TEntity, bool>> filter)
        where TEntity : BaseEntity => await DbContext.Set<TEntity>().AsNoTracking().Where(filter).ToListAsync();

    public readonly P2PDexDbContext DbContext;
}