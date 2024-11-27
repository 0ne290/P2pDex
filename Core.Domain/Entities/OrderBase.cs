using Core.Domain.Enums;

namespace Core.Domain.Entities;

public abstract class OrderBase : EntityBase
{
    protected OrderBase(string guid, OrderStatus status, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo, Trader seller,
        (decimal SellerToExchanger, decimal ExpectedExchangerToMiners) fee, string? transactionHash,
        Trader? buyer, string? buyersWalletAddress) : base(guid)
    {
        Status = status;
        Crypto = crypto;
        CryptoAmount = cryptoAmount;
        Fiat = fiat;
        CryptoToFiatExchangeRate = cryptoToFiatExchangeRate;
        FiatAmount = cryptoAmount * cryptoToFiatExchangeRate;
        PaymentMethodInfo = paymentMethodInfo;
        Seller = seller;
        SellerGuid = seller.Guid;
        Fee = fee;
        TransactionHash = transactionHash;
        Buyer = buyer;
        BuyerGuid = buyer?.Guid;
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

    public string? TransactionHash { get; protected set; }

    public Trader? Buyer { get; protected set; }

    public string? BuyerGuid { get; protected set; }

    public string? BuyersWalletAddress { get; protected set; }
}