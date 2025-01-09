using MediatR;

namespace Core.Application.General.Commands;

public class GetExchangerAccountAddressCommand : IRequest<CommandResult>;