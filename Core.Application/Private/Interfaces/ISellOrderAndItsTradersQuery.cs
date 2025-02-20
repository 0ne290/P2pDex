using System.Linq.Expressions;
using Core.Application.Api.SellOrder.Get;
using Core.Domain.Entities;

namespace Core.Application.Private.Interfaces;

public interface ISellOrderAndItsTradersQuery
{
    /// <summary>
    /// Возвращаемые сущности НЕ добавляются в отслеживатель.
    /// </summary>
    Task<SellOrderAndItsTradersDto?> Execute(Expression<Func<SellOrder, bool>> filter);
}