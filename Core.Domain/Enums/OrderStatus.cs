namespace Core.Domain.Enums;

public enum OrderStatus
{
    Created,
    BuyerResponded,
    SellerResponded,
    BuyerConfirmed,
    BuyerAndSellerConfirmed,
    FrozenForDurationOfDispute,
    //Cancelled,
    Completed
}
