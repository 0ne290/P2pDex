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
        if (!EthereumTransactionHashRegex.IsMatch(sellerToExchangerTransferTransactionHash))
            throw new DevelopmentErrorException("Seller to exchanger transfer transaction hash is invalid.");

        SellerGuid = sellerGuid;
        SellerToExchangerTransferTransactionHash = sellerToExchangerTransferTransactionHash;

        BuyerGuid = null;
        BuyerAccountAddress = null;
    }

    public override void ConfirmSellerToExchangerTransferTransaction()
    {
        if (Status != OrderStatus.Created)
            throw new DevelopmentErrorException("Status is invalid.");
        
        Status = OrderStatus.SellerToExchangerTransferTransactionConfirmed;
    }

    public void RespondByBuyer(Guid buyerGuid, string buyerAccountAddress)
    {
        if (Status != OrderStatus.SellerToExchangerTransferTransactionConfirmed)
            throw new InvariantViolationException("Status is invalid.");
        if (SellerGuid.Equals(buyerGuid))
            throw new InvariantViolationException("Trader cannot be both a seller and a buyer at the same time.");
        if (!EthereumAccountAddressRegex.IsMatch(buyerAccountAddress))
            throw new DevelopmentErrorException("Buyer wallet address is invalid.");

        BuyerGuid = buyerGuid;
        BuyerAccountAddress = buyerAccountAddress;
        Status = OrderStatus.RespondedByBuyer;
    }

    public override void ConfirmTransferFiatToSellerByBuyer()
    {
        if (Status != OrderStatus.RespondedByBuyer)
            throw new InvariantViolationException("Status is invalid.");

        Status = OrderStatus.TransferFiatToSellerConfirmedByBuyer;
    }

    public Guid SellerGuid { get; private set; }

    public string SellerToExchangerTransferTransactionHash { get; private set; }

    public Guid? BuyerGuid { get; private set; }

    public string? BuyerAccountAddress { get; private set; }
}