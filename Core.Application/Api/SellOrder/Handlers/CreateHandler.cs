using Core.Application.Api.SellOrder.Commands;
using Core.Application.Private.Configurations;
using Core.Application.Private.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Exceptions;
using MediatR;

namespace Core.Application.Api.SellOrder.Handlers;

public class CreateSellOrderHandler : IRequestHandler<CreateSellOrderCommand, IDictionary<string, object>>
{
    public CreateSellOrderHandler(IBlockchain blockchain, IUnitOfWork unitOfWork, ExchangerConfiguration exchangerConfiguration)
    {
        _blockchain = blockchain;
        _unitOfWork = unitOfWork;
        _exchangerConfiguration = exchangerConfiguration;
    }

    public async Task<IDictionary<string, object>> Handle(CreateSellOrderCommand request, CancellationToken _)
    {
        var sellerToExchangerFee = request.CryptoAmount * _exchangerConfiguration.FeeRate;
        var exchangerToMinersFee = _blockchain.GetTransferTransactionFee(DateTime.Now).Value;
        var order = new Domain.Entities.SellOrder(Guid.NewGuid(), request.Crypto, request.CryptoAmount, request.Fiat,
            request.CryptoToFiatExchangeRate, request.PaymentMethodInfo, sellerToExchangerFee, exchangerToMinersFee,
            request.SellerId, request.TransferTransactionHash);

        if (!await _unitOfWork.Repository.Exists<Trader>(t => t.Id == request.SellerId))
            throw new InvariantViolationException("Seller does not exists.");
        if (await _unitOfWork.Repository.Exists<Domain.Entities.SellOrder>(o =>
                o.SellerToExchangerTransferTransactionHash == request.TransferTransactionHash)/* ||
            await _unitOfWork.Repository.Exists<Domain.Entities.BuyOrder>(o =>
                o.SellerToExchangerTransferTransactionHash == request.TransferTransactionHash)*/)
            throw new InvariantViolationException("Transaction has already been used to pay for the order.");

        await _unitOfWork.Repository.Add(order);
        await _unitOfWork.SaveAllTrackedEntities();
        
        IDictionary<string, object> ret = new Dictionary<string, object>
        {
            ["guid"] = order.Guid,
            ["status"] = order.Status
        };

        return ret;
    }

    private readonly IBlockchain _blockchain;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ExchangerConfiguration _exchangerConfiguration;
}