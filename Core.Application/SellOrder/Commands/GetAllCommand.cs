using MediatR;

namespace Core.Application.SellOrder.Commands;

public class GetAllSellOrdersCommand : IRequest<CommandResult>;