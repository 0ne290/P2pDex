using MediatR;

namespace Core.Application.UseCases.General.Commands;

public class GetExchangerAccountAddressCommand : IRequest<IDictionary<string, object>>;