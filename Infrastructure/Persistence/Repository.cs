using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class Repository : IRepository
{
    public Repository(P2PDexContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add<TEntity>(TEntity entity) where TEntity : BaseEntity =>
        await _dbContext.Set<TEntity>().AddAsync(entity);

    public async Task<TEntity?> TryGetByGuid<TEntity>(Guid guid) where TEntity : BaseEntity =>
        await _dbContext.Set<TEntity>().FirstOrDefaultAsync(e => e.Guid.Equals(guid));

    private readonly P2PDexContext _dbContext;
}