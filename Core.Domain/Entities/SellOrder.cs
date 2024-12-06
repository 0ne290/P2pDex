using Core.Domain.Enums;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public class SellOrder : BaseOrder
{
    public SellOrder(Guid guid, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo,
        (decimal SellerToExchanger, decimal ExchangerToMiners) fee, Trader seller,
        string sellerToExchangerTransferTransactionHash) : base(guid, crypto, cryptoAmount, fiat,
        cryptoToFiatExchangeRate, paymentMethodInfo, fee)
    {
        if (string.IsNullOrWhiteSpace(sellerToExchangerTransferTransactionHash))
            throw new InvariantViolationException("Seller to exchanger transfer transaction hash is invalid.");

        Seller = seller;
        SellerToExchangerTransferTransactionHash = sellerToExchangerTransferTransactionHash;

        Buyer = null;
        BuyerWalletAddress = null;
    }

    public SellOrder(Guid guid, OrderStatus status, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo,
        (decimal SellerToExchanger, decimal ExchangerToMiners) fee, Trader seller,
        string sellerToExchangerTransferTransactionHash, Trader? buyer, string? buyerWalletAddress,
        string? exchangerToBuyerTransferTransactionHash) : this(guid, crypto, cryptoAmount, fiat,
        cryptoToFiatExchangeRate, paymentMethodInfo, fee, seller, sellerToExchangerTransferTransactionHash)
    {
        if (!Enum.IsDefined(status) || status == OrderStatus.SellerResponded)
            throw new InvariantViolationException("Status is invalid.");

        if (status == OrderStatus.Completed)
        {
            Buyer = buyer ?? throw new InvariantViolationException("Status and buyer is invalid.");
            BuyerWalletAddress = buyerWalletAddress == null || string.IsNullOrWhiteSpace(buyerWalletAddress)
                ? throw new InvariantViolationException("Status and buyer wallet address is invalid.")
                : buyerWalletAddress;

            ExchangerToBuyerTransferTransactionHash =
                exchangerToBuyerTransferTransactionHash == null ||
                string.IsNullOrWhiteSpace(exchangerToBuyerTransferTransactionHash)
                    ? throw new InvariantViolationException(
                        "Status and exchanger to buyer transfer transaction hash is invalid.")
                    : exchangerToBuyerTransferTransactionHash;

            Status = status;
        }
        else if (status != OrderStatus.Created)
        {
            Buyer = buyer ?? throw new InvariantViolationException("Status and buyer is invalid.");
            BuyerWalletAddress = buyerWalletAddress == null || string.IsNullOrWhiteSpace(buyerWalletAddress)
                ? throw new InvariantViolationException("Status and buyer wallet address is invalid.")
                : buyerWalletAddress;
            
            Status = status;
        }
    }

    public void Respond(Trader buyer, string buyerWalletAddress)
    {
        if (Status != OrderStatus.Created)
            throw new InvariantViolationException("Status is invalid.");
        if (string.IsNullOrWhiteSpace(buyerWalletAddress))
            throw new InvariantViolationException("Buyer wallet address is invalid.");

        Buyer = buyer;
        BuyerWalletAddress = buyerWalletAddress;
        Status = OrderStatus.BuyerResponded;
    }

    public void BuyerConfirm()
    {
        if (Status != OrderStatus.BuyerResponded)
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
                Seller.IncrementSuccessfulOrdersAsSeller();
                Buyer!.IncrementSuccessfulOrdersAsBuyer();
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

    public Trader Seller { get; }

    public string SellerToExchangerTransferTransactionHash { get; }

    public Trader? Buyer { get; private set; }

    public string? BuyerWalletAddress { get; private set; }
}