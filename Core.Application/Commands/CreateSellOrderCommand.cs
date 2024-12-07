using Core.Domain.Enums;
using FluentResults;
using MediatR;

namespace Core.Application.Commands;

public class CreateSellOrderCommand : IRequest<Result<(Guid, OrderStatus)>>
{
    public required Cryptocurrency Crypto { get; init; }

    public required decimal CryptoAmount { get; init; }

    public required FiatCurrency Fiat { get; init; }

    public required decimal CryptoToFiatExchangeRate { get; init; }

    public required string PaymentMethodInfo { get; init; }
}