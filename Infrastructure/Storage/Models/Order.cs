using Entities = Core.Domain.Entities;
using Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Infrastructure.Storage.Models;

public class Order : ModelBase
{
    public Entities.SellOrder ToEntity()
    {
        var entity = new Entities.SellOrder(System.Guid.Parse(Guid), Crypto, CryptoAmount, Fiat, CryptoToFiatExchangeRate,
            PaymentMethodInfo, (SellerToExchangerFee, ExchangerToMinersFee));
        
        var statusHistory = StatusHistory.Split(",").Select(s => Enum.Parse<OrderStatus>(s));

        foreach (var status in statusHistory)
        {
            switch (Status)
            {
                case OrderStatus.BuyerResponded:
                    entity.BuyerRespond(Buyer, BuyerWalletAddress);
                    break;
                case OrderStatus.SellerResponded:
                    entity.SellerRespond(Seller, SellerTransferTransactionHash);
                    break;
                case OrderStatus.BuyerConfirmed:
                    entity.BuyerConfirm();
                    break;
                case OrderStatus.SellerConfirmed:
                    entity.SellerConfirm();
                    break;
                case OrderStatus.Frozen:
                    entity.SellerDeny(Dispute);// TODO: Спор, вероятно, надо убрать из этого метода, т. к. соблюдение того, что в случае отказа продавца должен создасться спор, ссылающийся на заказ - это ответственность Application Layer. Если сущность X не вызывает методы сущности Y, то она НЕ ДОЛЖНА ИСПОЛЬЗОВАТЬ ЭТУ СУЩНОСТЬ. Вывод: неиспользуемые ссылки в Domain Layer - это бессмыслица
                case OrderStatus.Completed:
                    entity.Complete();// Вот тут опасный момент, т. к. этот метод под капотом вызывает методы сущностей Trader, тем самым будет выполнятся дублирующее нежелательное изменение состояния сущностей Trader
                    break;
            }
        }
    }

    public static Order FromEntity(Entities.SellOrder entity) => new()
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
        StatusHistory = string.Join(",", entity.StatusHistory)
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

    public required string StatusHistory { get; init; }
}
