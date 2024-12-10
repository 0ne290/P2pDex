using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

public interface IRepository
{
    Task Add<TEntity>(TEntity entity) where TEntity : BaseEntity;
    
    Task<bool> Exists<TEntity>(Predicate<TEntity> condition) where TEntity : BaseEntity;
    
    Task<TEntity> GetByGuid<TEntity>(Guid guid) where TEntity : BaseEntity;
}