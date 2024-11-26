using Core.Domain.Enums;

namespace Core.Application.Commands;

public class CreateSellOrderCommand
{
    public required Cryptocurrency Crypto { get; init; }

    public required decimal CryptoAmount { get; init; }

    public required FiatCurrency Fiat { get; init; }

    public required decimal CryptoToFiatExchangeRate { get; init; }

    public required string PaymentMethodInfo { get; init; }

    public required string SellerGuid { get; init; }
}