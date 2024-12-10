using Core.Domain.Enums;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public class SellOrder : BaseOrder
{
    public SellOrder(Guid guid, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo,
        (decimal SellerToExchanger, decimal ExchangerToMiners) fee, Guid sellerGuid,
        string sellerToExchangerTransferTransactionHash) : base(guid, crypto, cryptoAmount, fiat,
        cryptoToFiatExchangeRate, paymentMethodInfo, fee)
    {
        if (string.IsNullOrWhiteSpace(sellerToExchangerTransferTransactionHash))
            throw new DevelopmentErrorException("Seller to exchanger transfer transaction hash is invalid.");

        SellerGuid = sellerGuid;
        SellerToExchangerTransferTransactionHash = sellerToExchangerTransferTransactionHash;

        BuyerGuid = null;
        BuyerWalletAddress = null;
    }

    /// <summary>
    /// Небезопасный конструктор для восстановления объектов из их состояния. Состояния могут храниться, например,
    /// в БД, файлах и т. д. Конструктор не проверяет получаемое состояние и, соответственно, восстановленный объект
    /// может не удовлетворять бизнес-инвариантам. Для избежания этого любое сохранение состояний должно происходить
    /// только из кода через интерфейсы репозиториев.
    /// </summary>
    public SellOrder(Guid guid, OrderStatus status, Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, decimal fiatAmount, string paymentMethodInfo,
        (decimal SellerToExchanger, decimal ExchangerToMiners) fee, Guid sellerGuid,
        string sellerToExchangerTransferTransactionHash, Guid? buyerGuid, string? buyerWalletAddress,
        string? exchangerToBuyerTransferTransactionHash) : base(guid, status, crypto, cryptoAmount, fiat,
        cryptoToFiatExchangeRate, fiatAmount, paymentMethodInfo, fee, exchangerToBuyerTransferTransactionHash)
    {
        SellerGuid = sellerGuid;
        SellerToExchangerTransferTransactionHash = sellerToExchangerTransferTransactionHash;
        BuyerGuid = buyerGuid;
        BuyerWalletAddress = buyerWalletAddress;
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

    public Guid SellerGuid { get; }

    public string SellerToExchangerTransferTransactionHash { get; }

    public Guid? BuyerGuid { get; private set; }

    public string? BuyerWalletAddress { get; private set; }
}