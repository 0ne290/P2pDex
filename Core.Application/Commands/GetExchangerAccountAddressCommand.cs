using MediatR;

namespace Core.Application.Commands;

public class GetExchangerAccountAddressCommand : IRequest<CommandResult>;