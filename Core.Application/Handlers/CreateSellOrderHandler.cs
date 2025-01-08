using Core.Application.Commands;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using MediatR;

namespace Core.Application.Handlers;

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
        var order = new SellOrder(Guid.NewGuid(), request.Crypto, request.CryptoAmount, request.Fiat,
            request.CryptoToFiatExchangeRate, request.PaymentMethodInfo, sellerToExchangerFee, exchangerToMinersFee,
            request.SellerGuid, request.TransferTransactionHash);

        if (!await _unitOfWork.Repository.Exists<Trader>(t => t.Guid.Equals(request.SellerGuid)))
            throw new InvariantViolationException("Seller does not exists.");
        if (await _unitOfWork.Repository.Exists<SellOrder>(o =>
                o.SellerToExchangerTransferTransactionHash == request.TransferTransactionHash))
            throw new InvariantViolationException("Transaction has already been used to pay for the order.");

        await _unitOfWork.Repository.Add(order);
        await _unitOfWork.SaveAllTrackedEntities();

        return new CommandResult(new { guid = order.Guid, status = order.Status.ToString() });
    }

    private readonly IBlockchain _blockchain;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ExchangerConfiguration _exchangerConfiguration;
}