using MediatR;

namespace Core.Application.UseCases.SellOrder.Commands;

public class GetAllSellOrdersCommand : IRequest<IDictionary<string, object>>;