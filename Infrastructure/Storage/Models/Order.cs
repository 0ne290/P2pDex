using System.ComponentModel.DataAnnotations;
using Core.Domain.Enums;
using Infrastructure.Storage.Constants;

namespace Infrastructure.Storage.Models;

public class Order : ModelBase
{
    public required OrderType Type { get; init; }
    
    public required OrderStatus Status { get; init; }

    public required Cryptocurrency Crypto { get; init; }

    public required decimal CryptoAmount { get; init; }

    public required FiatCurrency Fiat { get; init; }

    public required decimal CryptoToFiatExchangeRate { get; init; }

    public required decimal FiatAmount { get; init; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string PaymentMethodInfo { get; init; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string SellerGuid { get; init; }

    public required decimal SellerToExchangerFee { get; init; }

    public required decimal ExchangerToMinersFee { get; init; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string? TransferTransactionHash { get; init; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string? BuyerGuid { get; init; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string? BuyersWalletAddress { get; init; }
}