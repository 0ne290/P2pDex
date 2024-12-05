namespace Core.Domain.Enums;

public enum OrderStatus
{
    Created,
    BuyerResponded,
    SellerResponded,
    BuyerConfirmed,
    SellerConfirmed,
    FrozenForDurationOfDispute,
    //Cancelled,
    Completed
}
