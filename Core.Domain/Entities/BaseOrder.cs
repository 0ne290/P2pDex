using Core.Domain.Enums;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public abstract class BaseOrder : BaseEntity
{
    protected BaseOrder(Guid guid, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo,
        (decimal SellerToExchanger, decimal ExchangerToMiners) fee) : base(guid)
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
        if (fee.SellerToExchanger < 0)
            throw new DevelopmentErrorException("Seller to exchanger fee is invalid.");
        if (fee.ExchangerToMiners <= 0)
            throw new DevelopmentErrorException("Exchanger to miners fee is invalid.");

        Status = OrderStatus.Created;
        Crypto = crypto;
        CryptoAmount = cryptoAmount;
        Fiat = fiat;
        CryptoToFiatExchangeRate = cryptoToFiatExchangeRate;
        FiatAmount = cryptoAmount * cryptoToFiatExchangeRate;
        PaymentMethodInfo = paymentMethodInfo;
        Fee = fee;
        ExchangerToBuyerTransferTransactionHash = null;
    }

    protected BaseOrder(Guid guid, OrderStatus status, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, decimal fiatAmount, string paymentMethodInfo,
        (decimal SellerToExchanger, decimal ExchangerToMiners) fee,
        string? exchangerToBuyerTransferTransactionHash) : base(guid)
    {
        Status = status;
        Crypto = crypto;
        CryptoAmount = cryptoAmount;
        Fiat = fiat;
        CryptoToFiatExchangeRate = cryptoToFiatExchangeRate;
        FiatAmount = fiatAmount;
        PaymentMethodInfo = paymentMethodInfo;
        Fee = fee;
        ExchangerToBuyerTransferTransactionHash = null;
        ExchangerToBuyerTransferTransactionHash = exchangerToBuyerTransferTransactionHash;
    }

    public OrderStatus Status { get; protected set; }

    public Cryptocurrency Crypto { get; }

    public decimal CryptoAmount { get; }

    public FiatCurrency Fiat { get; }

    public decimal CryptoToFiatExchangeRate { get; }

    public decimal FiatAmount { get; }

    public string PaymentMethodInfo { get; }

    public (decimal SellerToExchanger, decimal ExchangerToMiners) Fee { get; }

    public string? ExchangerToBuyerTransferTransactionHash { get; protected set; }
}