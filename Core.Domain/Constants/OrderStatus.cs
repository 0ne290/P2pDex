namespace Core.Domain.Constants;

public enum OrderStatus
{
    Created,
    SellerToExchangerTransferTransactionConfirmed,
    RespondedByBuyer,
    //RespondedBySeller,
    TransferFiatToSellerConfirmedByBuyer,
    ReceiptFiatFromBuyerConfirmedBySeller,
    //FrozenForDurationOfDispute,
    //Cancelled,
    ExchangerToBuyerTransferTransactionConfirmed
}