using Core.Enums;

namespace Core.Models;

public class Order : ModelBase
{
    public static Order CreateSellOrder(string guid, Trader seller, string transactionHash, Cryptocurrency crypto,
        decimal cryptoAmount, FiatCurrency fiat, decimal cryptoToFiatExchangeRate, string paymentMethodInfo) => new(
        guid, OrderType.Sell, OrderStatus.WaitingForConfirmationOfTheTransactionOfTransferOfCryptocurrencyToTheEscrowAccountByTheSeller, seller, transactionHash, null, null, crypto, cryptoAmount, fiat,
        cryptoToFiatExchangeRate, paymentMethodInfo);

    public static Order CreateCopy(Order order) => new(order.Guid, order.Type, order.Status, order.Seller,
        order.TransactionHash, order.Buyer, order.BuyersWalletAddress, order.Crypto, order.CryptoAmount, order.Fiat,
        order.CryptoToFiatExchangeRate, order.PaymentMethodInfo);

    private Order(string guid, OrderType type, OrderStatus status, Trader? seller, string? transactionHash, Trader? buyer,
        string? buyersWalletAddress, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo) : base(guid)
    {
        Type = type;
        Status = status;
        Seller = seller;
        SellerGuid = seller?.Guid;
        TransactionHash = transactionHash;
        Buyer = buyer;
        BuyerGuid = buyer?.Guid;
        BuyersWalletAddress = buyersWalletAddress;
        Crypto = crypto;
        CryptoAmount = cryptoAmount;
        Fiat = fiat;
        CryptoToFiatExchangeRate = cryptoToFiatExchangeRate;
        FiatAmount = cryptoAmount * cryptoToFiatExchangeRate;
        PaymentMethodInfo = paymentMethodInfo;
    }

    public void ConfirmTransaction() => Status = Type == OrderType.Sell
        ? OrderStatus.WaitingForBuyersResponse
        : OrderStatus.WaitingForSellerToConfirmReceiptOfFiatCurrencyFromBuyer;

    public void AssignBuyer(Trader buyer)
    {
        Status = OrderStatus
            .WaitingForConfirmationOfTheTransactionOfTransferOfCryptocurrencyToTheEscrowAccountByTheSeller;
        Buyer = buyer;
        BuyerGuid = buyer.Guid;
    }
    
    public OrderType Type { get; }

    public OrderStatus Status { get; private set; }

    public Trader? Seller { get; }

    public string? SellerGuid { get; }

    public string? TransactionHash { get; }

    public Trader? Buyer { get; protected set; }

    public string? BuyerGuid { get; private set; }

    public string? BuyersWalletAddress { get; private set; }

    public Cryptocurrency Crypto { get; }

    public decimal CryptoAmount { get; }

    public FiatCurrency Fiat { get; }

    public decimal CryptoToFiatExchangeRate { get; }

    public decimal FiatAmount { get; }

    public string PaymentMethodInfo { get; }
}