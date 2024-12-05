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

        Status = OrderStatus.Created;
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
    }
    
    public void BuyerRespond(Trader buyer, string buyerWalletAddress)
    {
        Status = Status switch
        {
            OrderStatus.Created => OrderStatus.BuyerResponded,
            OrderStatus.SellerResponded => OrderStatus.BuyerAndSellerResponded,
            _ => throw new InvariantViolationException("Status is invalid.")
        };

        Buyer = buyer;
        BuyerGuid = buyer.Guid;
        BuyerWalletAddress = buyerWalletAddress;
    }

    public void SellerRespond(Trader seller, string transferTransactionHash)
    {
        Status = Status switch
        {
            OrderStatus.Created => OrderStatus.SellerResponded,
            OrderStatus.BuyerResponded => OrderStatus.BuyerAndSellerResponded,
            _ => throw new InvariantViolationException("Status is invalid.")
        };

        Seller = seller;
        SellerGuid = seller.Guid;
        SellerTransferTransactionHash = transferTransactionHash;
    }
    
    public void BuyerConfirm()
    {
        if (Status != OrderStatus.BuyerAndSellerResponded)
            throw new InvariantViolationException("Status is invalid.");
        
        Status = OrderStatus.BuyerConfirmed;
    }
    
    public void SellerConfirm()
    {
        if (Status != OrderStatus.BuyerConfirmed)
            throw new InvariantViolationException("Status is invalid.");
        
        Status = OrderStatus.BuyerAndSellerConfirmed;
    }

    public void SellerDeny(Dispute dispute)
    {
        if (Status != OrderStatus.BuyerConfirmed)
            throw new InvariantViolationException("Status is invalid.");
        if (dispute.OrderGuid != Guid)
            throw new InvariantViolationException("Dispute is invalid.");
        
        Status = OrderStatus.FrozenForDurationOfDispute;
    }
    
    public void Complete()
    {
        switch (Status)
        {
            case OrderStatus.FrozenForDurationOfDispute:
                Status = OrderStatus.Completed;
                break;
            case OrderStatus.BuyerAndSellerConfirmed:
                Status = OrderStatus.Completed;
                Seller!.IncrementSuccessfulOrdersAsSeller();
                Buyer!.IncrementSuccessfulOrdersAsBuyer();
                break;
            default:
                throw new InvariantViolationException("Status is invalid.");
        }
    }

    //public void Cancel()
    //{
    //    if (Status is OrderStatus.Completed or OrderStatus.Cancelled)
    //        throw new InvariantViolationException("Status is invalid.");
    //    
    //    Status = OrderStatus.Cancelled;
    //}
    
    public OrderStatus Status { get; private set; }
    
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
}
