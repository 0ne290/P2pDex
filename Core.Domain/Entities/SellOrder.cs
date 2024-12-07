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
            throw new DevelopmentErrorException("Seller to exchanger transfer transaction hash is invalid.");

        Seller = seller;
        SellerToExchangerTransferTransactionHash = sellerToExchangerTransferTransactionHash;

        Buyer = null;
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
        (decimal SellerToExchanger, decimal ExchangerToMiners) fee, Trader seller,
        string sellerToExchangerTransferTransactionHash, Trader? buyer, string? buyerWalletAddress,
        string? exchangerToBuyerTransferTransactionHash) : base(guid, status, crypto, cryptoAmount, fiat,
        cryptoToFiatExchangeRate, fiatAmount, paymentMethodInfo, fee, exchangerToBuyerTransferTransactionHash)
    {
        Seller = seller;
        SellerToExchangerTransferTransactionHash = sellerToExchangerTransferTransactionHash;
        Buyer = buyer;
        BuyerWalletAddress = buyerWalletAddress;
    }

    public void Respond(Trader buyer, string buyerWalletAddress)
    {
        if (Status != OrderStatus.Created)
            throw new InvariantViolationException("Status is invalid.");
        if (string.IsNullOrWhiteSpace(buyerWalletAddress))
            throw new DevelopmentErrorException("Buyer wallet address is invalid.");

        Buyer = buyer;
        BuyerWalletAddress = buyerWalletAddress;
        Status = OrderStatus.BuyerResponded;
    }

    public void Confirm(Trader trader)
    {
        Status = Status switch
        {
            OrderStatus.BuyerResponded when trader.Equals(Buyer) => OrderStatus.BuyerConfirmed,
            OrderStatus.BuyerResponded => throw new InvariantViolationException("Trader is not a buyer."),
            OrderStatus.BuyerConfirmed when trader.Equals(Seller) => OrderStatus.BuyerAndSellerConfirmed,
            OrderStatus.BuyerConfirmed => throw new InvariantViolationException("Trader is not a seller."),
            _ => throw new InvariantViolationException("Status is invalid.")
        };
    }

    public void Deny(Trader trader)
    {
        if (Status != OrderStatus.BuyerConfirmed)
            throw new InvariantViolationException("Status is invalid.");
        if (!trader.Equals(Seller))
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