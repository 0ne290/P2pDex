using Core.Domain.Entities;
using Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Storage.Models;

public class Order : ModelBase
{
    public Order(OrderBase o)
    {
        Guid = o.Guid;
        Type = o is SellOrder ? OrderType.Sell : OrderType.Buy;
        Status = o.Status;
        Crypto = o.Crypto;
        CryptoAmount = o.CryptoAmount;
        Fiat = o.Fiat;
        CryptoToFiatExchangeRate = o.CryptoToFiatExchangeRate;
        FiatAmount = o.FiatAmount;
        PaymentMethodInfo = o.PaymentMethodInfo;
        SellerGuid = o.SellerGuid;
        SellerToExchangerFee = o.Fee.SellerToExchanger;
        ExchangerToMinersFee = o.Fee.ExpectedExchangerToMiners;
        TransferTransactionHash = o.TransferTransactionHash;
        BuyerGuid = o.BuyerGuid;
        BuyersWalletAddress = o.BuyersWalletAddress;
    }

    public Order() { }

    public async Task<OrderBase> ToEntity(P2pDexContext dbContext)
    {
        var trader = await dbContext.Traders.FirstAsync(t => t.Guid == SellerGuid);

        if (Type == OrderType.Sell)
            return new SellOrder(Guid, Crypto, CryptoAmount, Fiat, CryptoToFiatExchangeRate, PaymentMethodInfo,
                new Core.Domain.Entities.Trader(trader.Guid, trader.Name), (SellerToExchangerFee, ExchangerToMinersFee));

        throw new Exception("Buy orders are not supported yet.");
        //return new BuyOrder();
    }
    
    public OrderType Type { get; init; }
    
    public OrderStatus Status { get; init; }

    public Cryptocurrency Crypto { get; init; }

    public decimal CryptoAmount { get; init; }

    public FiatCurrency Fiat { get; init; }

    public decimal CryptoToFiatExchangeRate { get; init; }

    public decimal FiatAmount { get; init; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string PaymentMethodInfo { get; init; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string SellerGuid { get; init; }

    public decimal SellerToExchangerFee { get; init; }

    public decimal ExchangerToMinersFee { get; init; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? TransferTransactionHash { get; init; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? BuyerGuid { get; init; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? BuyersWalletAddress { get; init; }
}