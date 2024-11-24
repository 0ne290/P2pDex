namespace Infrastructure.Storage.Models;

public class Order : ModelBase
{
    public OrderType Type { get; }

    public OrderStatus Status { get; private set; }

    public Trader? Seller { get; }

    public string? SellerGuid { get; }

    public string? TransactionHash { get; }

    public Trader? Buyer { get; protected set; }

    public string? BuyerGuid { get; private set; }

    public string? BuyersWalletAddress { get; private set; }

    public Cryptocurrency Crypto { get; }

    public decimal CryptoAmount { get; }

    public FiatCurrency Fiat { get; }

    public decimal CryptoToFiatExchangeRate { get; }

    public decimal FiatAmount { get; }

    public string PaymentMethodInfo { get; }

    public decimal? SellerToExchangerFee { get; }

    public decimal? ExchangerToMinersExpectedFee { get; }

    public decimal? ExchangerExpectedProfit { get; }

    public decimal? ExchangerToMinersActualFee { get; private set; }

    public decimal? ExchangerActualProfit { get; private set; }
}