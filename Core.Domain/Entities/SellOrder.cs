using Core.Domain.Enums;

namespace Core.Domain.Entities;

public class SellOrder : OrderBase
{
    public SellOrder(string guid, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo, Trader seller, decimal feeFromSeller) : base(guid,
        OrderStatus.WaitingConfirmBySellerOfCryptocurrencyTransferTransaction,
        crypto, cryptoAmount, fiat, cryptoToFiatExchangeRate, paymentMethodInfo)
    {
        Seller = seller;
        SellerGuid = seller.Guid;
        FeeFromSeller = feeFromSeller;
        TransactionHash = null;
        Buyer = null;
        BuyerGuid = null;
        BuyersWalletAddress = null;
    }

    //public static SellOrder Copy(SellOrder order) => new(order.Guid, order.Type, order.Status, order.Seller,
    //    order.TransactionHash, order.Buyer, order.BuyersWalletAddress, order.Crypto, order.CryptoAmount, order.Fiat,
    //    order.CryptoToFiatExchangeRate, order.PaymentMethodInfo, order.SellerToExchangerFee,
    //    order.ExchangerToMinersExpectedFee, order.ExchangerToMinersActualFee);

    public void ConfirmBySellerOfCryptocurrencyTransferTransaction(string transactionHash)
    {
        TransactionHash = transactionHash;
        Status = OrderStatus.WaitingConfirmByBlockchainOfCryptocurrencyTransferTransaction;
    }
    
    public void ConfirmByBlockchainOfCryptocurrencyTransferTransaction()
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

    public void Complete()
    {
        Status = OrderStatus.Completed;
    }

    public Trader Seller { get; }

    public string SellerGuid { get; }
    
    public decimal FeeFromSeller { get; }

    public string? TransactionHash { get; private set; }

    public Trader? Buyer { get; protected set; }

    public string? BuyerGuid { get; private set; }

    public string? BuyersWalletAddress { get; private set; }
}