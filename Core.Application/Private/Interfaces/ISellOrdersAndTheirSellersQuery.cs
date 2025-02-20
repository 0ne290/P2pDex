using System.Collections;
using System.Linq.Expressions;
using Core.Domain.Entities;

namespace Core.Application.Private.Interfaces;

public interface ISellOrdersAndTheirSellersQuery
{
    /// <summary>
    /// Возвращаемые сущности НЕ добавляются в отслеживатель.
    /// </summary>
    Task<ICollection> Execute(Expression<Func<SellOrder, bool>> filter);
}