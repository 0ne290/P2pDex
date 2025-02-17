using MediatR;

namespace Core.Application.Api.General.Commands;

public class GetExchangerAccountAddressCommand : IRequest<IDictionary<string, object>>;