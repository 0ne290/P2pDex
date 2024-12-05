using Entities = Core.Domain.Entities;
using Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Infrastructure.Storage.Models;

public class Order : ModelBase
{
    public Entities.Order ToEntity()
    {
        var entity = new Entities.Order(System.Guid.Parse(Guid), Crypto, CryptoAmount, Fiat, CryptoToFiatExchangeRate,
            PaymentMethodInfo, (SellerToExchangerFee, ExchangerToMinersFee));
        
        sw
    }

    public static Order FromEntity(Entities.Order entity) => new()
    {
        Guid = entity.Guid.ToString(),
        Status = entity.CurrentStatus,
        Crypto = entity.Crypto,
        CryptoAmount = entity.CryptoAmount,
        Fiat = entity.Fiat,
        CryptoToFiatExchangeRate = entity.CryptoToFiatExchangeRate,
        FiatAmount = entity.FiatAmount,
        PaymentMethodInfo = entity.PaymentMethodInfo,
        SellerToExchangerFee = entity.Fee.SellerToExchanger,
        ExchangerToMinersFee = entity.Fee.ExchangerToMiners,
        SellerGuid = entity.SellerGuid.ToString(),
        SellerTransferTransactionHash = entity.SellerTransferTransactionHash,
        BuyerGuid = entity.BuyerGuid.ToString(),
        BuyerWalletAddress = entity.BuyerWalletAddress,
    };
    
    public required OrderStatus Status { get; init; }
    
    public required Cryptocurrency Crypto { get; init; }

    public required decimal CryptoAmount { get; init; }

    public required FiatCurrency Fiat { get; init; }

    public required decimal CryptoToFiatExchangeRate { get; init; }

    public required decimal FiatAmount { get; init; }

    public required string PaymentMethodInfo { get; init; }

    public required decimal SellerToExchangerFee { get; init; }
    
    public required decimal ExchangerToMinersFee { get; init; }

    public Trader? Seller { get; init; }

    public string? SellerGuid { get; init; }

    public string? SellerTransferTransactionHash { get; init; }

    public Trader? Buyer { get; init; }

    public string? BuyerGuid { get; init; }

    public string? BuyerWalletAddress { get; init; }
}