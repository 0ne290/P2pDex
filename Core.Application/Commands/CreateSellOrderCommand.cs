using Core.Domain.Enums;

namespace Core.Application.Commands;

public class CreateSellOrderCommand
{
    public required string SellerGuid { get; init; }

    public required string TransactionHash { get; init; }

    public required Cryptocurrency Crypto { get; init; }

    public required decimal CryptoAmount { get; init; }

    public required FiatCurrency Fiat { get; init; }

    public required decimal CryptoToFiatExchangeRate { get; init; }

    public required string PaymentMethodInfo { get; init; }

    public required decimal SellerToExchangerFee { get; init; }

    public required decimal ExchangerToMinersExpectedFee { get; init; }
}