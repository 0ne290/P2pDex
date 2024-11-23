namespace Core.Domain.Enums;

public enum OrderStatus
{
    WaitingForBuyersResponse,
    WaitingForSellersResponse,
    WaitingForConfirmationOfTheTransactionOfTransferOfCryptocurrencyToTheEscrowAccountByTheSeller,
    WaitingForSellerToConfirmReceiptOfFiatCurrencyFromBuyer,
    Cancelled,
    Completed
}