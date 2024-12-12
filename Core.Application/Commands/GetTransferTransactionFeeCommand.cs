using MediatR;

namespace Core.Application.Commands;

public class GetTransferTransactionFeeCommand : IRequest<CommandResult>;