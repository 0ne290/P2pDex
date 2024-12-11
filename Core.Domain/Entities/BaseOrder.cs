using Core.Domain.Enums;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public abstract class BaseOrder : BaseEntity
{
    protected BaseOrder() { }
    
    protected BaseOrder(Guid guid, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo,
        decimal sellerToExchangerFee, decimal exchangerToMinersFee) : base(guid)
    {
        if (!Enum.IsDefined(crypto))
            throw new InvariantViolationException("Crypto is invalid.");
        if (cryptoAmount <= 0)
            throw new InvariantViolationException("Crypto amount is invalid.");
        if (!Enum.IsDefined(fiat))
            throw new InvariantViolationException("Fiat is invalid.");
        if (cryptoToFiatExchangeRate <= 0)
            throw new InvariantViolationException("Crypto to fiat exchange rate is invalid.");
        if (string.IsNullOrWhiteSpace(paymentMethodInfo) || paymentMethodInfo.Length > 64)
            throw new InvariantViolationException("Payment method info is invalid.");
        if (sellerToExchangerFee < 0)
            throw new DevelopmentErrorException("Seller to exchanger fee is invalid.");
        if (exchangerToMinersFee <= 0)
            throw new DevelopmentErrorException("Exchanger to miners fee is invalid.");

        Status = OrderStatus.Created;
        Crypto = crypto;
        CryptoAmount = cryptoAmount;
        Fiat = fiat;
        CryptoToFiatExchangeRate = cryptoToFiatExchangeRate;
        FiatAmount = cryptoAmount * cryptoToFiatExchangeRate;
        PaymentMethodInfo = paymentMethodInfo;
        SellerToExchangerFee = sellerToExchangerFee;
        ExchangerToMinersFee = exchangerToMinersFee;
        ExchangerToBuyerTransferTransactionHash = null;
    }

    public OrderStatus Status { get; protected set; }

    public Cryptocurrency Crypto { get; private set; }

    public decimal CryptoAmount { get; private set; }

    public FiatCurrency Fiat { get; private set; }

    public decimal CryptoToFiatExchangeRate { get; private set; }

    public decimal FiatAmount { get; private set; }

    public string PaymentMethodInfo { get; private set; }

    public decimal SellerToExchangerFee { get; private set; }
    
    public decimal ExchangerToMinersFee { get; private set; }

    public string? ExchangerToBuyerTransferTransactionHash { get; protected set; }
}