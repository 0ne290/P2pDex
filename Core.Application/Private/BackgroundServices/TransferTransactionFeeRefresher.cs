using Core.Application.Private.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Application.Private.BackgroundServices;

public class TransferTransactionFeeRefresher : BackgroundService
{
    public TransferTransactionFeeRefresher(IBlockchain blockchain, ILogger<TransferTransactionFeeRefresher> logger, int intervalInMs)
    {
        _blockchain = blockchain;
        _logger = logger;
        _intervalInMs = intervalInMs;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _blockchain.RefreshTransferTransactionFee(DateTime.Now, _intervalInMs);
            var fee = _blockchain.GetTransferTransactionFee(DateTime.Now);

            _logger.LogInformation(
                "TransferTransactionFeeRefresher: transfer transaction fee is refreshed. Fee: {Fee}; Time to update in milliseconds: {TimeToUpdateInMs}.",
                fee.Value, fee.TimeToUpdateInMs);
 
            await Task.Delay(_intervalInMs, stoppingToken);
        }
    }

    private readonly IBlockchain _blockchain;

    private readonly ILogger<TransferTransactionFeeRefresher> _logger;

    private readonly int _intervalInMs;
}