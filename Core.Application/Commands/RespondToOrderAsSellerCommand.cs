using Core.Domain.Enums;
using FluentResults;
using MediatR;

namespace Core.Application.Commands;

public class RespondToOrderAsSellerCommand : IRequest<Result<(Guid, OrderStatus)>>
{
    public required string OrderGuid { get; init; }
    
    public required string SellerGuid { get; init; }
    
    public required string TransferTransactionHash { get; init; }
}