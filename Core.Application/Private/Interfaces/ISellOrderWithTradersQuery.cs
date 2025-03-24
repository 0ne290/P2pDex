using Core.Application.Api.SellOrder.Get;

namespace Core.Application.Private.Interfaces;

public interface ISellOrderWithTradersQuery
{
    /// <summary>
    /// Возвращаемые сущности НЕ добавляются в отслеживатель.
    /// </summary>
    Task<SellOrderWithTraders?> Execute(Guid orderGuid);
}