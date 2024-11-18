using Core.Enums;

namespace Core.Models;

public class SellOrder : OrderBase
{
    public SellOrder(Trader seller, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo) : base(OrderStatus.WaitingForBuyersResponse, seller,
        null, crypto, cryptoAmount, fiat, cryptoToFiatExchangeRate, paymentMethodInfo) { }

    public void AssignBuyer(Trader buyer)
    {
        Status = OrderStatus
            .WaitingForConfirmationOfTheTransactionOfTransferOfCryptocurrencyToTheEscrowAccountByTheSeller;
        Buyer = buyer;
        BuyerGuid = buyer.Guid;
    }
}