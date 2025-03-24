using System.Collections;
using System.Linq.Expressions;
using Core.Application.Api.SellOrder.Commands;
using Core.Domain.Entities;

namespace Core.Application.Private.Interfaces;

public interface ISellOrdersWithSellersQuery
{
    /// <summary>
    /// Возвращаемые сущности НЕ добавляются в отслеживатель.
    /// </summary>
    Task<ICollection<SellOrderWithSeller>> Execute(Expression<Func<SellOrder, bool>> filter);
}