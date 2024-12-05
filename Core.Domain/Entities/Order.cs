using Core.Domain.Enums;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public class Order : EntityBase
{
    public Order(Guid guid, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
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
            throw new InvariantViolationException("Seller to exchanger fee is invalid.");
        if (fee.ExchangerToMiners <= 0)
            throw new InvariantViolationException("Exchanger to miners fee is invalid.");

        CurrentStatus = OrderStatus.Created;
        Crypto = crypto;
        CryptoAmount = cryptoAmount;
        Fiat = fiat;
        CryptoToFiatExchangeRate = cryptoToFiatExchangeRate;
        FiatAmount = cryptoAmount * cryptoToFiatExchangeRate;
        PaymentMethodInfo = paymentMethodInfo;
        Fee = fee;
        Seller = null;
        SellerGuid = null;
        SellerTransferTransactionHash = null;
        Buyer = null;
        BuyerGuid = null;
        BuyerWalletAddress = null;

        StatusHistory = new List<OrderStatus>(6) { CurrentStatus };
    }
    
    public void BuyerRespond(Trader buyer, string buyerWalletAddress)
    {
        if (CurrentStatus is OrderStatus.Created or OrderStatus.SellerResponded)
            CurrentStatus = OrderStatus.BuyerResponded;
        else
            throw new InvariantViolationException("Current status is invalid.");
        
        Buyer = buyer;
        BuyerGuid = buyer.Guid;
        BuyerWalletAddress = buyerWalletAddress;
        
        StatusHistory.Add(CurrentStatus);
    }

    public void SellerRespond(Trader seller, string transferTransactionHash)
    {
        if (CurrentStatus is OrderStatus.Created or OrderStatus.BuyerResponded)
            CurrentStatus = OrderStatus.SellerResponded;
        else
            throw new InvariantViolationException("Current status is invalid.");

        Seller = seller;
        SellerGuid = seller.Guid;
        SellerTransferTransactionHash = transferTransactionHash;
        
        StatusHistory.Add(CurrentStatus);
    }
    
    public void BuyerConfirm()
    {
        if ((StatusHistory[^1] == OrderStatus.BuyerResponded && StatusHistory[^2] == OrderStatus.SellerResponded) ||
            (StatusHistory[^1] == OrderStatus.SellerResponded && StatusHistory[^2] == OrderStatus.BuyerResponded))
            throw new InvariantViolationException("Status history is invalid.");
        
        CurrentStatus = OrderStatus.BuyerConfirmed;
        
        StatusHistory.Add(CurrentStatus);
    }
    
    public void SellerConfirm()
    {
        if (CurrentStatus != OrderStatus.BuyerConfirmed)
            throw new InvariantViolationException("Current status is invalid.");
        
        CurrentStatus = OrderStatus.SellerConfirmed;
        
        StatusHistory.Add(CurrentStatus);
    }

    public void SellerDeny(Dispute dispute)
    {
        if (CurrentStatus != OrderStatus.BuyerConfirmed)
            throw new InvariantViolationException("Current status is invalid.");
        if (!Equals(dispute.Order))
            throw new InvariantViolationException("Dispute is invalid.");
        
        CurrentStatus = OrderStatus.FrozenForDurationOfDispute;
        
        StatusHistory.Add(CurrentStatus);
    }
    
    public void Complete()
    {
        switch (CurrentStatus)
        {
            case OrderStatus.FrozenForDurationOfDispute:
                CurrentStatus = OrderStatus.Completed;
                break;
            case OrderStatus.SellerConfirmed:
                CurrentStatus = OrderStatus.Completed;
                Seller!.IncrementSuccessfulOrdersAsSeller();
                Buyer!.IncrementSuccessfulOrdersAsBuyer();
                break;
            default:
                throw new InvariantViolationException("CurrentStatus is invalid.");
        }
        
        StatusHistory.Add(CurrentStatus);
    }

    //public void Cancel()
    //{
    //    if (CurrentStatus is OrderStatus.Completed or OrderStatus.Cancelled)
    //        throw new InvariantViolationException("CurrentStatus is invalid.");
    //    
    //    CurrentStatus = OrderStatus.Cancelled;
    //}
    
    public OrderStatus CurrentStatus { get; private set; }
    
    public Cryptocurrency Crypto { get; }

    public decimal CryptoAmount { get; }

    public FiatCurrency Fiat { get; }

    public decimal CryptoToFiatExchangeRate { get; }

    public decimal FiatAmount { get; }

    public string PaymentMethodInfo { get; }
    
    public (decimal SellerToExchanger, decimal ExchangerToMiners) Fee { get; }

    public Trader? Seller { get; private set; }

    public Guid? SellerGuid { get; private set; }

    public string? SellerTransferTransactionHash { get; private set; }

    public Trader? Buyer { get; private set; }

    public Guid? BuyerGuid { get; private set; }

    public string? BuyerWalletAddress { get; private set; }

    public List<OrderStatus> StatusHistory { get; }
}