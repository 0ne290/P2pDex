using Core.Domain.Constants;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public class BuyOrder : BaseOrder
{
    private BuyOrder() { }

    public BuyOrder(Guid guid, string crypto, decimal cryptoAmount, string fiat, decimal cryptoToFiatExchangeRate,
        string paymentMethodInfo, decimal sellerToExchangerFee, decimal exchangerToMinersFee, Guid buyerGuid,
        string buyerAccountAddress) : base(guid, crypto, cryptoAmount, fiat, cryptoToFiatExchangeRate,
        paymentMethodInfo, sellerToExchangerFee, exchangerToMinersFee)
    {
        if (!EthereumAccountAddressRegex.IsMatch(buyerAccountAddress))
            throw new DevelopmentErrorException("Buyer wallet address is invalid.");

        BuyerGuid = buyerGuid;
        BuyerAccountAddress = buyerAccountAddress;

        SellerGuid = null;
        SellerToExchangerTransferTransactionHash = null;
    }

    public void RespondBySeller(Guid sellerGuid, string sellerToExchangerTransferTransactionHash)
    {
        if (Status != OrderStatus.Created)
            throw new InvariantViolationException("Status is invalid.");
        if (BuyerGuid.Equals(sellerGuid))
            throw new InvariantViolationException("Trader cannot be both a buyer and a seller at the same time.");
        if (!EthereumTransactionHashRegex.IsMatch(sellerToExchangerTransferTransactionHash))
            throw new DevelopmentErrorException("Seller to exchanger transfer transaction hash is invalid.");

        SellerGuid = sellerGuid;
        SellerToExchangerTransferTransactionHash = sellerToExchangerTransferTransactionHash;
        Status = OrderStatus.RespondedBySeller;
    }
    
    public override void ConfirmSellerToExchangerTransferTransaction()
    {
        if (Status != OrderStatus.RespondedBySeller)
            throw new DevelopmentErrorException("Status is invalid.");
        
        Status = OrderStatus.SellerToExchangerTransferTransactionConfirmed;
    }

    public override void ConfirmTransferFiatToSellerByBuyer()
    {
        if (Status != OrderStatus.SellerToExchangerTransferTransactionConfirmed)
            throw new InvariantViolationException("Status is invalid.");

        Status = OrderStatus.TransferFiatToSellerConfirmedByBuyer;
    }
    
    public Guid BuyerGuid { get; private set; }
    
    public string BuyerAccountAddress { get; private set; }

    public Guid? SellerGuid { get; private set; }

    public string? SellerToExchangerTransferTransactionHash { get; private set; }
}