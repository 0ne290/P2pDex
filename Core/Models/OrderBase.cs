using Core.Enums;

namespace Core.Models;

public abstract class OrderBase : ModelBase
{
    public required OrderStatus Status { get; init; }
    
    public required Trader Seller { get; init; }
    
    public required string SellerGuid { get; init; }
    
    public required Trader Buyer { get; init; }
    
    public required string BuyerGuid { get; init; }
    
    public required Cryptocurrency Crypto { get; init; }
    
    public required decimal CryptoAmount { get; init; }
    
    public required FiatCurrency Fiat { get; init; }
    
    public required decimal CryptoToFiatExchangeRate { get; init; }
    
    public decimal FiatAmount { get; }
    
    public required string PaymentMethodInfo { get; init; }
    
    public required string JsonOfChatContent { get; init; }
}