using System.Dynamic;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Exceptions;

namespace Core.Domain.Services;

public class SellOrderManager
{
    public SellOrderManager(IBlockchain blockchain, ITraderStorage traderStorage)
    {
        _blockchain = blockchain;
        _traderStorage = traderStorage;
    }

    public async Task<SellOrder> Create(Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo, Guid sellerGuid,
        string sellerToExchangerTransferTransactionHash)
    {
        var seller = await _traderStorage.TryGetByGuid(sellerGuid);

        if (seller == null)
            throw new InvariantViolationException("Seller does not exists..");
        
        var transaction = await _blockchain.TryGetConfirmedTransaction(sellerToExchangerTransferTransactionHash);

        if (transaction == null)
            throw new InvariantViolationException("Transaction either does not exist, has not yet been confirmed, or has been rejected.");
        if (transaction.To != _blockchain.ExchangerAccountAddress)
            throw new InvariantViolationException(
                "Cryptocurrency was transferred to the wrong address. For a refund, contact the recipient.");

        var expectedCryptoAmount =
            order.CryptoAmount + order.Fee.SellerToExchanger + order.Fee.ExchangerToMiners;

        if (expectedCryptoAmount != transactionInfo.Amount)
        {
            var refundTransactionHash = await _blockchain.SendTransferTransaction(transactionInfo.From,
                transactionInfo.Amount - order.Fee.ExchangerToMiners);

            return Result.Fail(
                $"Amount of cryptocurrency transferred should have been {expectedCryptoAmount}. Cryptocurrency refund transaction with the collected transfer fee has already been accepted for processing. Wait for confirmation by blockchain. Refund transaction hash: {refundTransactionHash}.");
        }
        
        var order = new SellOrder(Guid.NewGuid(), crypto, cryptoAmount, fiat, cryptoToFiatExchangeRate, paymentMethodInfo, CalculateFee(cryptoAmount), seller)
    }

    private async Task<(decimal SellerToExchanger, decimal ExchangerToMiners)> CalculateFee(decimal cryptoAmount) =>
        (cryptoAmount * feeRate, await _blockchain.GetTransferTransactionFee());

    private readonly IBlockchain _blockchain;

    private readonly ITraderStorage _traderStorage;
}