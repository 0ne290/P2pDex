using Core.Domain.Enums;

namespace Core.Domain.Entities;

public class Order : EntityBase
{
    private Order(string guid, OrderType type, OrderStatus status, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo, Trader seller,
        (decimal SellerToExchanger, decimal ExpectedExchangerToMiners) fee, string? transferTransactionHash,
        Trader? buyer, string? buyersWalletAddress) : base(guid)
    {
        Status = status;
        Type = type;
        Crypto = crypto;
        CryptoAmount = cryptoAmount;
        Fiat = fiat;
        CryptoToFiatExchangeRate = cryptoToFiatExchangeRate;
        FiatAmount = cryptoAmount * cryptoToFiatExchangeRate;
        PaymentMethodInfo = paymentMethodInfo;
        Seller = seller;
        SellerGuid = seller.Guid;
        Fee = fee;
        TransferTransactionHash = transferTransactionHash;
        Buyer = buyer;
        BuyerGuid = buyer?.Guid;
        BuyersWalletAddress = buyersWalletAddress;
    }

    public static Order CreateSellOrder(string guid, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo, Trader seller,
        (decimal SellerToExchanger, decimal ExpectedExchangerToMiners) fee) => new(guid, OrderType.Sell
        OrderStatus.WaitingConfirmBySellerOfCryptocurrencyTransferTransaction, crypto, cryptoAmount, fiat,
        cryptoToFiatExchangeRate, paymentMethodInfo, seller, fee, null, null, null) { }

    public void ConfirmBySellerOfCryptocurrencyTransferTransaction(string transferTransactionHash)
    {
        TransferTransactionHash = transferTransactionHash;
        Status = OrderStatus.WaitingConfirmByBlockchainOfCryptocurrencyTransferTransaction;
    }

    public void ConfirmByBlockchainOfCryptocurrencyTransferTransaction()
    {
        if (Type == OrderType.Sell)
            Status = OrderStatus.WaitingForBuyersResponse;
        else
            Status = OrderStatus.WaitingBySellerOfConfirmFiatCurrencyReceipt
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

    public void Cancel()
    {
        Status = OrderStatus.Cancelled;
    }

    public OrderType Type { get; }

    public OrderStatus Status { get; protected set; }

    public Cryptocurrency Crypto { get; }

    public decimal CryptoAmount { get; }

    public FiatCurrency Fiat { get; }

    public decimal CryptoToFiatExchangeRate { get; }

    public decimal FiatAmount { get; }

    public string PaymentMethodInfo { get; }
    
    public Trader Seller { get; }

    public string SellerGuid { get; }
    
    public (decimal SellerToExchanger, decimal ExpectedExchangerToMiners) Fee { get; }

    public string? TransferTransactionHash { get; protected set; }

    public Trader? Buyer { get; protected set; }

    public string? BuyerGuid { get; protected set; }

    public string? BuyersWalletAddress { get; protected set; }
}
