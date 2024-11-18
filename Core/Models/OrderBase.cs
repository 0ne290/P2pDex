using Core.Enums;

namespace Core.Models;

public abstract class OrderBase : ModelBase
{
    public OrderBase(OrderStatus status, Trader? seller, Trader? buyer, Cryptocurrency crypto, decimal cryptoAmount,
        FiatCurrency fiat, decimal cryptoToFiatExchangeRate, string paymentMethodInfo)
    {
        Status = status;
        Seller = seller;
        SellerGuid = seller?.Guid;
        Buyer = buyer;
        BuyerGuid = buyer?.Guid;
        Crypto = crypto;
        CryptoAmount = cryptoAmount;
        Fiat = fiat;
        CryptoToFiatExchangeRate = cryptoToFiatExchangeRate;
        FiatAmount = cryptoAmount * cryptoToFiatExchangeRate;
        PaymentMethodInfo = paymentMethodInfo;
    }

    public OrderStatus Status { get; protected set; }
    
    public Trader? Seller { get; }
    
    public string? SellerGuid { get; }
    
    public Trader? Buyer { get; protected set; }
    
    public string? BuyerGuid { get; protected set; }
    
    public Cryptocurrency Crypto { get; }
    
    public decimal CryptoAmount { get; }
    
    public FiatCurrency Fiat { get; }
    
    public decimal CryptoToFiatExchangeRate { get; }
    
    public decimal FiatAmount { get; }
    
    public string PaymentMethodInfo { get; }
}