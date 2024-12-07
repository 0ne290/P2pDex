using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;

namespace Core.Domain.Services;

public class Exchanger
{
    public Exchanger(IBlockchain blockchain, ITraderStorage traderStorage, string walletAddress, decimal feeRate)
    {
        _blockchain = blockchain;
        _traderStorage = traderStorage;
        _walletAddress = walletAddress;
        _feeRate = feeRate;
    }

    public async Task<SellOrder> CreateSellOrder(Cryptocurrency crypto, decimal cryptoAmount, FiatCurrency fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo, Guid sellerGuid,
        string sellerToExchangerTransferTransactionHash)
    {
        var seller = await _traderStorage.TryGetByGuid(sellerGuid);

        if (seller == null)
            throw new InvariantViolationException("Seller does not exists.");

        var transaction = await _blockchain.TryGetConfirmedTransactionByHash(sellerToExchangerTransferTransactionHash);

        if (transaction == null)
            throw new InvariantViolationException(
                "Transaction either does not exist, has not yet been confirmed, or has been rejected.");
        if (transaction.To != _walletAddress)
            throw new InvariantViolationException(
                "Cryptocurrency was transferred to the wrong address. For a refund, contact the recipient.");

        var fee = await CalculateFee(cryptoAmount);
        var expectedCryptoAmount = cryptoAmount + fee.SellerToExchanger + fee.ExchangerToMiners;

        if (expectedCryptoAmount != transaction.Amount)
        {
            var refundTransactionHash = await _blockchain.SendTransferTransaction(_walletAddress, transaction.From,
                transaction.Amount - fee.ExchangerToMiners);

            throw new InvariantViolationException(
                $"Amount of cryptocurrency transferred should have been {expectedCryptoAmount}. Cryptocurrency refund transaction with the collected transfer fee has already been accepted for processing. Wait for confirmation by blockchain. Refund transaction hash: {refundTransactionHash}.");
        }

        return new SellOrder(Guid.NewGuid(), crypto, cryptoAmount, fiat, cryptoToFiatExchangeRate, paymentMethodInfo,
            fee, seller, sellerToExchangerTransferTransactionHash);
    }

    private async Task<(decimal SellerToExchanger, decimal ExchangerToMiners)> CalculateFee(decimal cryptoAmount) =>
        (cryptoAmount * _feeRate, (await _blockchain.TransferTransactionFee).Value);

    private readonly IBlockchain _blockchain;

    private readonly ITraderStorage _traderStorage;

    private readonly string _walletAddress;

    private readonly decimal _feeRate;
}