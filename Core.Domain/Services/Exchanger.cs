using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;

namespace Core.Domain.Services;

public class Exchanger
{
    public Exchanger(IBlockchain blockchain, IRepository repository, decimal feeRate)
    {
        _blockchain = blockchain;
        _repository = repository;
        _feeRate = feeRate;
    }

    public (decimal FinalCryptoAmount, double RelevanceTimeInMs) CalculateFinalCryptoAmountForTransfer(decimal cryptoAmount)
    {
        var exchangerToMinersFee = _blockchain.TransferTransactionFee;
        var sellerToExchangerFee = cryptoAmount * _feeRate;

        var finalCryptoAmount = cryptoAmount + exchangerToMinersFee.Value + sellerToExchangerFee;

        return (finalCryptoAmount, exchangerToMinersFee.TimeToUpdateInMs);
    }

    public async Task<SellOrder> CreateSellOrder(string crypto, decimal cryptoAmount, string fiat,
        decimal cryptoToFiatExchangeRate, string paymentMethodInfo, Guid sellerGuid,
        string sellerToExchangerTransferTransactionHash)
    {
        if (!await _repository.Exists<Trader>(t => t.Guid.Equals(sellerGuid)))
            throw new InvariantViolationException("Seller does not exists.");

        var transaction = await _blockchain.TryGetConfirmedTransactionByHash(sellerToExchangerTransferTransactionHash);

        if (transaction == null)
            throw new InvariantViolationException(
                "Transaction either does not exist, has not yet been confirmed, or has been rejected.");
        
        if (transaction.To != _blockchain.AccountAddress)
            throw new InvariantViolationException(
                "Cryptocurrency was transferred to the wrong address. For a refund, contact the recipient.");

        var fee = CalculateFee(cryptoAmount);
        var expectedCryptoAmount = cryptoAmount + fee.SellerToExchanger + fee.ExchangerToMiners;
        
        Console.WriteLine($"{expectedCryptoAmount} {transaction.Amount} {fee.ExchangerToMiners}");

        if (expectedCryptoAmount != transaction.Amount)
        {
            var refundTransactionHash = await _blockchain.SendTransferTransaction(transaction.From, transaction.Amount - fee.ExchangerToMiners);

            throw new InvariantViolationException(
                $"Amount of cryptocurrency transferred should have been {expectedCryptoAmount}. Cryptocurrency refund transaction with the collected transfer fee has already been accepted for processing. Wait for confirmation by blockchain. Refund transaction hash: {refundTransactionHash}.");
        }

        return new SellOrder(Guid.NewGuid(), crypto, cryptoAmount, fiat, cryptoToFiatExchangeRate, paymentMethodInfo,
            fee.SellerToExchanger, fee.ExchangerToMiners, sellerGuid, sellerToExchangerTransferTransactionHash);
    }

    private (decimal SellerToExchanger, decimal ExchangerToMiners) CalculateFee(decimal cryptoAmount) =>
        (cryptoAmount * _feeRate, _blockchain.TransferTransactionFee.Value);

    private readonly IBlockchain _blockchain;

    private readonly IRepository _repository;

    private readonly decimal _feeRate;
}