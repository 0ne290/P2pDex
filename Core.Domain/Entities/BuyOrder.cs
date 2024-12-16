/*using Core.Domain.Constants;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public class BuyOrder : BaseOrder
{
    public BuyOrder(Guid guid, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo,
        (decimal SellerToExchanger, decimal ExchangerToMiners) fee, Trader buyer, string buyerWalletAddress) : base(
        guid, crypto, cryptoAmount, fiat, cryptoToFiatExchangeRate, paymentMethodInfo, fee)
    {
        if (string.IsNullOrWhiteSpace(buyerWalletAddress))
            throw new InvariantViolationException("Buyer wallet address is invalid.");

        Buyer = buyer;
        BuyerAccountAddress = buyerWalletAddress;

        Seller = null;
        SellerToExchangerTransferTransactionHash = null;
    }

    public void Respond(Trader seller, string sellerToExchangerTransferTransactionHash)
    {
        if (Status != OrderStatus.Created)
            throw new InvariantViolationException("Status is invalid.");
        if (string.IsNullOrWhiteSpace(sellerToExchangerTransferTransactionHash))
            throw new InvariantViolationException("Seller to exchanger transfer transaction hash is invalid.");

        Seller = seller;
        SellerToExchangerTransferTransactionHash = sellerToExchangerTransferTransactionHash;
        Status = OrderStatus.SellerResponded;
    }

    public void BuyerConfirm()
    {
        if (Status != OrderStatus.SellerResponded)
            throw new InvariantViolationException("Status is invalid.");

        Status = OrderStatus.BuyerConfirmed;
    }

    public void SellerConfirm()
    {
        if (Status != OrderStatus.BuyerConfirmed)
            throw new InvariantViolationException("Status is invalid.");

        Status = OrderStatus.BuyerAndSellerConfirmed;
    }

    public void SellerDeny()
    {
        if (Status != OrderStatus.BuyerConfirmed)
            throw new InvariantViolationException("Status is invalid.");

        Status = OrderStatus.FrozenForDurationOfDispute;
    }

    public void Complete(string exchangerToBuyerTransferTransactionHash)
    {
        if (string.IsNullOrWhiteSpace(exchangerToBuyerTransferTransactionHash))
            throw new InvariantViolationException("Exchanger to buyer transfer transaction hash is invalid.");

        switch (Status)
        {
            case OrderStatus.FrozenForDurationOfDispute:
                break;
            case OrderStatus.BuyerAndSellerConfirmed:
                Seller!.IncrementSuccessfulOrdersAsSeller();
                Buyer.IncrementSuccessfulOrdersAsBuyer();
                break;
            default:
                throw new InvariantViolationException("Status is invalid.");
        }

        ExchangerToBuyerTransferTransactionHash = exchangerToBuyerTransferTransactionHash;
        Status = OrderStatus.Completed;
    }

    //public void Cancel()
    //{
    //    if (Status is OrderStatus.Completed or OrderStatus.Cancelled)
    //        throw new InvariantViolationException("Status is invalid.");
    //    
    //    Status = OrderStatus.Cancelled;
    //}

    public Trader Buyer { get; protected init; }

    public string BuyerAccountAddress { get; protected init; }

    public Trader? Seller { get; private set; }

    public string? SellerToExchangerTransferTransactionHash { get; private set; }
}*/