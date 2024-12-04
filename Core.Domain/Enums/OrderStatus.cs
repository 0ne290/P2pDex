namespace Core.Domain.Enums;

public enum OrderStatus
{
    Created,
    BuyerResponded,
    SellerResponded,
    BuyerAndSellerResponded,
    BuyerConfirmed,
    BuyerAndSellerConfirmed,
    FrozenForDurationOfDispute,
    Cancelled,
    Completed
}
