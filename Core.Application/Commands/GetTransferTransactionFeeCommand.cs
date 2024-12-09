using FluentResults;
using MediatR;

namespace Core.Application.Commands;

public class GetTransferTransactionFeeCommand : IRequest<Result<(decimal Value, double TimeToUpdateInMs)>>;