using Core.Enums;

namespace Core.Models;

public class Order
{
    public required OrderType Type { get; init; }
    
    public required Trader Initiator { get; init; }
    
    public required string InitiatorGuid { get; init; }
    
    public required Trader Responded { get; init; }
    
    public required string RespondedGuid { get; init; }
    
    public required OrderStatus Status { get; init; }
    
    public required Cryptocurrency Crypto { get; init; }
    
    public required decimal CryptoAmount { get; init; }
    
    public required FiatCurrency Fiat { get; init; }
    
    public required decimal CryptoToFiatExchangeRate { get; init; }
    
    public decimal FiatAmount { get; }
    
    public required string PaymentMethodInfo { get; init; }
    
    public required string JsonOfChatContent { get; init; }
}