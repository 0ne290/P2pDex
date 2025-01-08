using System.Linq.Expressions;
using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

/// <summary>
/// Если клиентский код изменит свойства отслеживаемой сущности, состояние этой сущности в отслеживателе автоматически
/// изменится на "Изменена".
/// </summary>
public interface IRepository
{
    /// <summary>
    /// Добавляет сущность в отслеживатель с состоянием "Добавлена".
    /// </summary>
    Task Add<TEntity>(TEntity entity) where TEntity : BaseEntity;

    /// <summary>
    /// Добавляет сущность в отслеживатель с состоянием "Изменена".
    /// </summary>
    void UpdateRange<TEntity>(IEnumerable<TEntity> updatedEntities) where TEntity : BaseEntity;
    
    /// <summary>
    /// Никак не влияет на отслеживатель.
    /// </summary>
    Task<bool> Exists<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity;
    
    /// <summary>
    /// Возвращаемая сущность добавляется в отслеживатель с состоянием "Без изменений".
    /// </summary>
    Task<TEntity?> TryGetByGuid<TEntity>(Guid guid) where TEntity : BaseEntity;
    
    /// <summary>
    /// Возвращаемые сущности НЕ добавляются в отслеживатель.
    /// </summary>
    Task<ICollection<TEntity>> GetAll<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : BaseEntity;
}