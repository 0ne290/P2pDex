using MediatR;

namespace Core.Application.Api.SellOrder.Commands;

public class GetAllSellOrdersCommand : IRequest<IDictionary<string, object>>;