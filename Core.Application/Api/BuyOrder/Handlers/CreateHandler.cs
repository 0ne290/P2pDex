/*using Core.Application.BuyOrder.Commands;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.BuyOrder.Handlers;

public class CreateBuyOrderHandler : IRequestHandler<CreateBuyOrderCommand, CommandResult>
{
    public CreateBuyOrderHandler(IBlockchain blockchain, IUnitOfWork unitOfWork, ExchangerConfiguration exchangerConfiguration)
    {
        _blockchain = blockchain;
        _unitOfWork = unitOfWork;
        _exchangerConfiguration = exchangerConfiguration;
    }

    public async Task<CommandResult> Handle(CreateBuyOrderCommand request, CancellationToken _)
    {
        var sellerToExchangerFee = request.CryptoAmount * _exchangerConfiguration.FeeRate;
        var exchangerToMinersFee = _blockchain.TransferTransactionFee.Value;
        var order = new Domain.Entities.BuyOrder(Guid.NewGuid(), request.Crypto, request.CryptoAmount, request.Fiat,
            request.CryptoToFiatExchangeRate, request.PaymentMethodInfo, sellerToExchangerFee, exchangerToMinersFee,
            request.BuyerGuid, request.BuyerAccountAddress);

        if (!await _unitOfWork.Repository.Exists<Trader>(t => t.Guid.Equals(request.BuyerGuid)))
            throw new InvariantViolationException("Buyer does not exists.");

        await _unitOfWork.Repository.Add(order);
        await _unitOfWork.SaveAllTrackedEntities();

        return new CommandResult(new { guid = order.Guid, status = order.Status.ToString() });
    }

    private readonly IBlockchain _blockchain;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ExchangerConfiguration _exchangerConfiguration;
}*/