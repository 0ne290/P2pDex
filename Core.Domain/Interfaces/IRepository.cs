using System.Linq.Expressions;
using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

public interface IRepository
{
    Task Add<TEntity>(TEntity entity) where TEntity : BaseEntity;
    
    Task<bool> Exists<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity;
    
    Task<TEntity?> TryGetByGuid<TEntity>(Guid guid) where TEntity : BaseEntity;
    
    Task<ICollection<TEntity>> GetAll<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity;
}