using Core.Domain.Enums;

namespace Core.Domain.Entities;

public class SellOrder : OrderBase
{
    public SellOrder(string guid, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo, Trader seller,
        (decimal SellerToExchanger, decimal ExpectedExchangerToMiners) fee) : base(guid,
        OrderStatus.WaitingConfirmBySellerOfCryptocurrencyTransferTransaction, crypto, cryptoAmount, fiat,
        cryptoToFiatExchangeRate, paymentMethodInfo, seller, fee, null, null, null) { }

    //public static SellOrder Copy(SellOrder order) => new(order.Guid, order.Type, order.Status, order.Seller,
    //    order.TransferTransactionHash, order.Buyer, order.BuyersWalletAddress, order.Crypto, order.CryptoAmount, order.Fiat,
    //    order.CryptoToFiatExchangeRate, order.PaymentMethodInfo, order.SellerToExchangerFee,
    //    order.ExchangerToMinersExpectedFee, order.ExchangerToMinersActualFee);

    public override void ConfirmByBlockchainOfCryptocurrencyTransferTransaction()
    {
        Status = OrderStatus.WaitingForBuyersResponse;
    }

    public void AssignBuyer(Trader buyer, string buyersWalletAddress)
    {
        Status = OrderStatus.WaitingBySellerOfConfirmFiatCurrencyReceipt;
        Buyer = buyer;
        BuyerGuid = buyer.Guid;
        BuyersWalletAddress = buyersWalletAddress;
    }
}