using Core.Domain.Enums;
using FluentResults;
using MediatR;

namespace Core.Application.Commands;

public class ConfirmBySellerOfCryptocurrencyTransferTransactionCommand : IRequest<Result<(string, OrderStatus)>>
{
    public required string SellerGuid { get; init; }
    
    public required string OrderGuid { get; init; }
    
    public required string TransactionHash { get; init; }
}