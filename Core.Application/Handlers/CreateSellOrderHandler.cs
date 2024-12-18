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

        var transaction = await _blockchain.TryGetConfirmedTransactionByHash(request.TransferTransactionHash);

        if (transaction == null)
            throw new InvariantViolationException(
                "Transaction either does not exist, has not yet been confirmed, or has been rejected.");
        if (transaction.To != _exchangerConfiguration.AccountAddress)
            throw new InvariantViolationException(
                "Cryptocurrency was transferred to the wrong address. For a refund, contact the recipient.");

        var expectedCryptoAmount = request.CryptoAmount + sellerToExchangerFee + exchangerToMinersFee;

        if (expectedCryptoAmount != transaction.Amount)
        {
            var refundTransactionHash =
                await _blockchain.SendTransferTransaction(_exchangerConfiguration.AccountAddress, transaction.From,
                    transaction.Amount - exchangerToMinersFee);

            throw new InvariantViolationException(
                $"Amount of cryptocurrency transferred should have been {expectedCryptoAmount}. Cryptocurrency refund transaction with the collected transfer fee has already been accepted for processing. Wait for confirmation by blockchain. Refund transaction hash: {refundTransactionHash}.");
        }

        await _unitOfWork.Repository.Add(order);
        await _unitOfWork.Save();

        return new CommandResult(new { guid = order.Guid, status = order.Status });
    }

    private readonly IBlockchain _blockchain;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ExchangerConfiguration _exchangerConfiguration;
}