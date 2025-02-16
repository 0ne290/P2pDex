using System.Text.RegularExpressions;
using Core.Domain.Constants;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public abstract partial class BaseOrder : BaseEntity
{
    protected BaseOrder() { }
    
    protected BaseOrder(Guid guid, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo,
        decimal sellerToExchangerFee, decimal exchangerToMinersFee) : base(guid)
    {
        if (!Enum.IsDefined(crypto))
            throw new InvariantViolationException("Crypto is invalid.");
        if (cryptoAmount <= 0)
            throw new DevelopmentErrorException("Crypto amount is invalid.");
        if (!Enum.IsDefined(fiat))
            throw new InvariantViolationException("Fiat is invalid.");
        if (cryptoToFiatExchangeRate <= 0)
            throw new InvariantViolationException("Crypto to fiat exchange rate is invalid.");
        if (string.IsNullOrWhiteSpace(paymentMethodInfo))
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
    
    public abstract void ConfirmSellerToExchangerTransferTransaction();

    public abstract void ConfirmTransferFiatToSellerByBuyer();
    
    //public void DenyReceiptFiatFromBuyerBySeller()
    //{
    //    if (Status != OrderStatus.TransferFiatToSellerConfirmedByBuyer)
    //        throw new InvariantViolationException("Status is invalid.");
//
    //    Status = OrderStatus.FrozenForDurationOfDispute;
    //}
    
    public void ConfirmReceiptFiatFromBuyerBySeller(string exchangerToBuyerTransferTransactionHash)
    {
        if (Status != OrderStatus.TransferFiatToSellerConfirmedByBuyer)
            throw new DevelopmentErrorException("Status is invalid.");
        if (!EthereumTransactionHashRegex.IsMatch(exchangerToBuyerTransferTransactionHash))
            throw new DevelopmentErrorException("Exchanger to buyer transfer transaction hash is invalid.");

        ExchangerToBuyerTransferTransactionHash = exchangerToBuyerTransferTransactionHash;
        Status = OrderStatus.ReceiptFiatFromBuyerConfirmedBySeller;
    }
    
    public void ConfirmExchangerToBuyerTransferTransaction()
    {
        switch (Status)
        {
            //case OrderStatus.FrozenForDurationOfDispute:
            //    break;
            case OrderStatus.ReceiptFiatFromBuyerConfirmedBySeller:
                //Seller.IncrementSuccessfulOrdersAsSeller();
                //Buyer!.IncrementSuccessfulOrdersAsBuyer();
                break;
            default:
                throw new DevelopmentErrorException("Status is invalid.");
        }
        
        Status = OrderStatus.ExchangerToBuyerTransferTransactionConfirmed;
    }
    
    public void Cancel()
    {
        if (Status is OrderStatus.ExchangerToBuyerTransferTransactionConfirmed or OrderStatus.Cancelled)
            throw new InvariantViolationException("Status is invalid.");
        
        Status = OrderStatus.Cancelled;
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
    
    protected static readonly Regex EthereumTransactionHashRegex = CreateEthereumTransactionHashRegex();

    protected static readonly Regex EthereumAccountAddressRegex = CreateEthereumAccountAddressRegex();
    
    [GeneratedRegex("^0x[0-9a-fA-F]{64}$")]
    private static partial Regex CreateEthereumTransactionHashRegex();

    [GeneratedRegex("^0x[0-9a-fA-F]{40}$")]
    private static partial Regex CreateEthereumAccountAddressRegex();
}