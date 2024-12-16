using Core.Domain.Constants;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public class SellOrder : BaseOrder
{
    private SellOrder() { }
    
    public SellOrder(Guid guid, string crypto, decimal cryptoAmount, string fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo,
        decimal sellerToExchangerFee, decimal exchangerToMinersFee, Guid sellerGuid,
        string sellerToExchangerTransferTransactionHash) : base(guid, crypto, cryptoAmount, fiat,
        cryptoToFiatExchangeRate, paymentMethodInfo, sellerToExchangerFee, exchangerToMinersFee)
    {
        if (string.IsNullOrWhiteSpace(sellerToExchangerTransferTransactionHash))
            throw new DevelopmentErrorException("Seller to exchanger transfer transaction hash is invalid.");

        SellerGuid = sellerGuid;
        SellerToExchangerTransferTransactionHash = sellerToExchangerTransferTransactionHash;

        BuyerGuid = null;
        BuyerWalletAddress = null;
    }

    public void Respond(Guid buyerGuid, string buyerWalletAddress)
    {
        if (Status != OrderStatus.Created)
            throw new InvariantViolationException("Status is invalid.");
        if (string.IsNullOrWhiteSpace(buyerWalletAddress))
            throw new DevelopmentErrorException("Buyer wallet address is invalid.");

        BuyerGuid = buyerGuid;
        BuyerWalletAddress = buyerWalletAddress;
        Status = OrderStatus.BuyerResponded;
    }

    public void Confirm(Guid traderGuid)
    {
        Status = Status switch
        {
            OrderStatus.BuyerResponded when traderGuid.Equals(BuyerGuid) => OrderStatus.BuyerConfirmed,
            OrderStatus.BuyerResponded => throw new InvariantViolationException("Trader is not a buyer."),
            OrderStatus.BuyerConfirmed when traderGuid.Equals(SellerGuid) => OrderStatus.BuyerAndSellerConfirmed,
            OrderStatus.BuyerConfirmed => throw new InvariantViolationException("Trader is not a seller."),
            _ => throw new InvariantViolationException("Status is invalid.")
        };
    }

    public void Deny(Guid traderGuid)
    {
        if (Status != OrderStatus.BuyerConfirmed)
            throw new InvariantViolationException("Status is invalid.");
        if (!traderGuid.Equals(SellerGuid))
            throw new InvariantViolationException("Trader is not a seller.");

        Status = OrderStatus.FrozenForDurationOfDispute;
    }

    public void Complete(string exchangerToBuyerTransferTransactionHash)
    {
        if (string.IsNullOrWhiteSpace(exchangerToBuyerTransferTransactionHash))
            throw new DevelopmentErrorException("Exchanger to buyer transfer transaction hash is invalid.");

        switch (Status)
        {
            case OrderStatus.FrozenForDurationOfDispute:
                break;
            case OrderStatus.BuyerAndSellerConfirmed:
                //Seller.IncrementSuccessfulOrdersAsSeller();
                //Buyer!.IncrementSuccessfulOrdersAsBuyer();
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

    public Guid SellerGuid { get; private set; }

    public string SellerToExchangerTransferTransactionHash { get; private set; }

    public Guid? BuyerGuid { get; private set; }

    public string? BuyerWalletAddress { get; private set; }
}