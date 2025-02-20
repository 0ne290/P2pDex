using System.Linq.Expressions;
using Core.Domain.Entities;

namespace Core.Application.Private.Interfaces;

public interface ISellOrderAndItsTradersQuery
{
    /// <summary>
    /// Возвращаемые сущности НЕ добавляются в отслеживатель.
    /// </summary>
    Task<object?> Execute(Expression<Func<SellOrder, bool>> filter);
}