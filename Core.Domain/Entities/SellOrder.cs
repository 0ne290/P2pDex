using Core.Domain.Constants;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public class SellOrder : BaseOrder
{
    private SellOrder() { }
    
    public SellOrder(Guid guid, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo,
        decimal sellerToExchangerFee, decimal exchangerToMinersFee, long sellerId,
        string sellerToExchangerTransferTransactionHash) : base(guid, crypto, cryptoAmount, fiat,
        cryptoToFiatExchangeRate, paymentMethodInfo, sellerToExchangerFee, exchangerToMinersFee)
    {
        if (!EthereumTransactionHashRegex.IsMatch(sellerToExchangerTransferTransactionHash))
            throw new DevelopmentErrorException("Seller to exchanger transfer transaction hash is invalid.");

        SellerId = sellerId;
        SellerToExchangerTransferTransactionHash = sellerToExchangerTransferTransactionHash;

        BuyerId = null;
        BuyerAccountAddress = null;
    }

    public override void ConfirmSellerToExchangerTransferTransaction()
    {
        if (Status != OrderStatus.Created)
            throw new DevelopmentErrorException("Status is invalid.");
        
        Status = OrderStatus.SellerToExchangerTransferTransactionConfirmed;
    }

    public void RespondByBuyer(long buyerId, string buyerAccountAddress)
    {
        if (Status != OrderStatus.SellerToExchangerTransferTransactionConfirmed)
            throw new InvariantViolationException("Status is invalid.");
        if (SellerId.Equals(buyerId))
            throw new InvariantViolationException("Trader cannot be both a seller and a buyer at the same time.");
        if (!EthereumAccountAddressRegex.IsMatch(buyerAccountAddress))
            throw new DevelopmentErrorException("Buyer wallet address is invalid.");

        BuyerId = buyerId;
        BuyerAccountAddress = buyerAccountAddress;
        Status = OrderStatus.RespondedByBuyer;
    }

    public override void ConfirmTransferFiatToSellerByBuyer()
    {
        if (Status != OrderStatus.RespondedByBuyer)
            throw new InvariantViolationException("Status is invalid.");

        Status = OrderStatus.TransferFiatToSellerConfirmedByBuyer;
    }

    public long SellerId { get; private set; }

    public string SellerToExchangerTransferTransactionHash { get; private set; }

    public long? BuyerId { get; private set; }

    public string? BuyerAccountAddress { get; private set; }
}