using Core.Domain.Enums;

namespace Core.Domain.Models;

public class Order : EntityBase
{
    public static Order CreateSellOrder(string guid, Trader seller, string transactionHash, Cryptocurrency crypto,
        decimal cryptoAmount, FiatCurrency fiat, decimal cryptoToFiatExchangeRate, string paymentMethodInfo,
        decimal sellerToExchangerFee, decimal exchangerToMinersExpectedFee) => new(guid, OrderType.Sell,
        OrderStatus.WaitingForConfirmationOfTheTransactionOfTransferOfCryptocurrencyToTheEscrowAccountByTheSeller,
        seller, transactionHash, null, null, crypto, cryptoAmount, fiat, cryptoToFiatExchangeRate, paymentMethodInfo,
        sellerToExchangerFee, exchangerToMinersExpectedFee, null);

    public static Order CreateCopy(Order order) => new(order.Guid, order.Type, order.Status, order.Seller,
        order.TransactionHash, order.Buyer, order.BuyersWalletAddress, order.Crypto, order.CryptoAmount, order.Fiat,
        order.CryptoToFiatExchangeRate, order.PaymentMethodInfo, order.SellerToExchangerFee,
        order.ExchangerToMinersExpectedFee, order.ExchangerToMinersActualFee);

    private Order(string guid, OrderType type, OrderStatus status, Trader? seller, string? transactionHash,
        Trader? buyer, string? buyersWalletAddress, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo, decimal? sellerToExchangerFee,
        decimal? exchangerToMinersExpectedFee, decimal? exchangerToMinersActualFee) : base(guid)
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
        SellerToExchangerFee = sellerToExchangerFee;
        ExchangerToMinersExpectedFee = exchangerToMinersExpectedFee;
        ExchangerExpectedProfit = sellerToExchangerFee - exchangerToMinersExpectedFee;
        ExchangerToMinersActualFee = exchangerToMinersActualFee;
        ExchangerActualProfit = sellerToExchangerFee - exchangerToMinersActualFee;
    }

    public void ConfirmTransaction() => Status = Type == OrderType.Sell
        ? OrderStatus.WaitingForBuyersResponse
        : OrderStatus.WaitingForSellerToConfirmReceiptOfFiatCurrencyFromBuyer;

    public void AssignBuyer(Trader buyer, string buyersWalletAddress)
    {
        Status = OrderStatus
            .WaitingForConfirmationOfTheTransactionOfTransferOfCryptocurrencyToTheEscrowAccountByTheSeller;
        Buyer = buyer;
        BuyerGuid = buyer.Guid;
        BuyersWalletAddress = buyersWalletAddress;
    }

    public void Complete(decimal exchangerToMinersActualFee)
    {
        Status = OrderStatus.Completed;
        ExchangerToMinersActualFee = exchangerToMinersActualFee;
        ExchangerActualProfit = SellerToExchangerFee - exchangerToMinersActualFee;
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

    public decimal? SellerToExchangerFee { get; }

    public decimal? ExchangerToMinersExpectedFee { get; }

    public decimal? ExchangerExpectedProfit { get; }

    public decimal? ExchangerToMinersActualFee { get; private set; }

    public decimal? ExchangerActualProfit { get; private set; }
}