namespace Core.Domain.Constants;

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
