using Core.Domain.Enums;
using MediatR;

namespace Core.Application.Commands;

public class CreateSellOrderCommand : IRequest<CommandResult>
{
    public required Cryptocurrency Crypto { get; init; }

    public required decimal CryptoAmount { get; init; }

    public required FiatCurrency Fiat { get; init; }

    public required decimal CryptoToFiatExchangeRate { get; init; }

    public required string PaymentMethodInfo { get; init; }
    
    public required Guid SellerGuid { get; init; }
    
    public required string TransferTransactionHash { get; init; }
}