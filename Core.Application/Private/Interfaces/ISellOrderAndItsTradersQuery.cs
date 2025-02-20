using Core.Application.Api.SellOrder.Get;

namespace Core.Application.Private.Interfaces;

public interface ISellOrderAndItsTradersQuery
{
    /// <summary>
    /// Возвращаемые сущности НЕ добавляются в отслеживатель.
    /// </summary>
    Task<SellOrderAndItsTradersDto?> Execute(Guid orderGuid);
}