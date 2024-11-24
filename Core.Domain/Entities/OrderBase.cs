using Core.Domain.Enums;

namespace Core.Domain.Entities;

public abstract class OrderBase : EntityBase
{
    protected OrderBase(string guid, OrderStatus status, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo) : base(guid)
    {
        Status = status;
        Crypto = crypto;
        CryptoAmount = cryptoAmount;
        Fiat = fiat;
        CryptoToFiatExchangeRate = cryptoToFiatExchangeRate;
        FiatAmount = cryptoAmount * cryptoToFiatExchangeRate;
        PaymentMethodInfo = paymentMethodInfo;
    }

    public OrderStatus Status { get; protected set; }

    public Cryptocurrency Crypto { get; }

    public decimal CryptoAmount { get; }

    public FiatCurrency Fiat { get; }

    public decimal CryptoToFiatExchangeRate { get; }

    public decimal FiatAmount { get; }

    public string PaymentMethodInfo { get; }
}