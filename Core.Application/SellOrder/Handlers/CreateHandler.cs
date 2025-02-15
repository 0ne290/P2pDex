using Core.Application.SellOrder.Commands;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.SellOrder.Handlers;

public class CreateSellOrderHandler : IRequestHandler<CreateSellOrderCommand, CommandResult>
{
    public CreateSellOrderHandler(IBlockchain blockchain, IUnitOfWork unitOfWork, ExchangerConfiguration exchangerConfiguration)
    {
        _blockchain = blockchain;
        _unitOfWork = unitOfWork;
        _exchangerConfiguration = exchangerConfiguration;
    }

    public async Task<CommandResult> Handle(CreateSellOrderCommand request, CancellationToken _)
    {
        var sellerToExchangerFee = request.CryptoAmount * _exchangerConfiguration.FeeRate;
        var exchangerToMinersFee = _blockchain.TransferTransactionFee.Value;
        var order = new Domain.Entities.SellOrder(Guid.NewGuid(), request.Crypto, request.CryptoAmount, request.Fiat,
            request.CryptoToFiatExchangeRate, request.PaymentMethodInfo, sellerToExchangerFee, exchangerToMinersFee,
            request.SellerId, request.TransferTransactionHash);

        if (!await _unitOfWork.Repository.Exists<Trader>(t => t.Id.Equals(request.SellerId)))
            throw new InvariantViolationException("Seller does not exists.");
        if (await _unitOfWork.Repository.Exists<Domain.Entities.SellOrder>(o =>
                o.SellerToExchangerTransferTransactionHash == request.TransferTransactionHash)/* ||
            await _unitOfWork.Repository.Exists<Domain.Entities.BuyOrder>(o =>
                o.SellerToExchangerTransferTransactionHash == request.TransferTransactionHash)*/)
            throw new InvariantViolationException("Transaction has already been used to pay for the order.");

        await _unitOfWork.Repository.Add(order);
        await _unitOfWork.SaveAllTrackedEntities();
        
        var ret = new Dictionary<string, object>
        {
            ["guid"] = order.Guid,
            ["status"] = order.Status.ToString()
        };

        return new CommandResult(ret);
    }

    private readonly IBlockchain _blockchain;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ExchangerConfiguration _exchangerConfiguration;
}