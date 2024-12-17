using System.Text.RegularExpressions;
using Core.Domain.Constants;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public partial class SellOrder : BaseOrder
{
    private SellOrder() { }
    
    public SellOrder(Guid guid, string crypto, decimal cryptoAmount, string fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo,
        decimal sellerToExchangerFee, decimal exchangerToMinersFee, Guid sellerGuid,
        string sellerToExchangerTransferTransactionHash) : base(guid, crypto, cryptoAmount, fiat,
        cryptoToFiatExchangeRate, paymentMethodInfo, sellerToExchangerFee, exchangerToMinersFee)
    {
        if (!EthereumTransactionHashRegex.IsMatch(sellerToExchangerTransferTransactionHash))
            throw new DevelopmentErrorException("Seller to exchanger transfer transaction hash is invalid.");

        SellerGuid = sellerGuid;
        SellerToExchangerTransferTransactionHash = sellerToExchangerTransferTransactionHash;

        BuyerGuid = null;
        BuyerAccountAddress = null;
    }

    public void Respond(Guid buyerGuid, string buyerAccountAddress)
    {
        if (Status != OrderStatus.Created)
            throw new InvariantViolationException("Status is invalid.");
        if (!EthereumAccountAddressRegex.IsMatch(buyerAccountAddress))
            throw new DevelopmentErrorException("Buyer wallet address is invalid.");

        BuyerGuid = buyerGuid;
        BuyerAccountAddress = buyerAccountAddress;
        Status = OrderStatus.BuyerResponded;
    }

    public void ConfirmByBuyer()
    {
        if (Status != OrderStatus.BuyerResponded)
            throw new InvariantViolationException("Status is invalid.");

        Status = OrderStatus.BuyerConfirmed;
    }

    public void ConfirmBySeller()
    {
        if (Status != OrderStatus.BuyerConfirmed)
            throw new InvariantViolationException("Status is invalid.");

        Status = OrderStatus.SellerConfirmed;
    }

    public void Deny()
    {
        if (Status != OrderStatus.BuyerConfirmed)
            throw new InvariantViolationException("Status is invalid.");

        Status = OrderStatus.FrozenForDurationOfDispute;
    }

    public void Complete(string exchangerToBuyerTransferTransactionHash)
    {
        if (!EthereumTransactionHashRegex.IsMatch(exchangerToBuyerTransferTransactionHash))
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

    public string? BuyerAccountAddress { get; private set; }
    
    private static readonly Regex EthereumTransactionHashRegex = CreateEthereumTransactionHashRegex();

    private static readonly Regex EthereumAccountAddressRegex = CreateEthereumAccountAddressRegex();
    
    [GeneratedRegex("^0x[0-9a-fA-F]{64}$")]
    private static partial Regex CreateEthereumTransactionHashRegex();

    [GeneratedRegex("^0x[0-9a-fA-F]{40}$")]
    private static partial Regex CreateEthereumAccountAddressRegex();
}
