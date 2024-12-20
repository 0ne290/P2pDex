using System.Linq.Expressions;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class Repository : IRepository
{
    public Repository(P2PDexDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add<TEntity>(TEntity entity) where TEntity : BaseEntity =>
        await _dbContext.Set<TEntity>().AddAsync(entity);

    public async Task<bool> Exists<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : BaseEntity =>
        await _dbContext.Set<TEntity>().AnyAsync(condition);

    public async Task<TEntity?> TryGetByGuid<TEntity>(Guid guid) where TEntity : BaseEntity =>
        await _dbContext.Set<TEntity>().FirstOrDefaultAsync(e => e.Guid.Equals(guid));

    private readonly P2PDexDbContext _dbContext;
}