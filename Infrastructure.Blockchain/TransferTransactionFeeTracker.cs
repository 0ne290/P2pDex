using System.Timers;
using Nethereum.Web3;
using Timer = System.Timers.Timer;

namespace Infrastructure.Blockchain;

public class TransferTransactionFeeTracker : IDisposable
{
    public TransferTransactionFeeTracker(Web3 web3, double intervalInMs)
    {
        _synchronizer = 0;

        _web3 = web3;
        
        UpdateFee(DateTime.Now).GetAwaiter().GetResult();
        
        _timer = new Timer { AutoReset = true, Enabled = false, Interval = intervalInMs };
        _timer.Elapsed += ElapsedEventHandler;
        _timer.Start();
    }

    private async void ElapsedEventHandler(object? _, ElapsedEventArgs args) =>
        await ExecuteConcurrently(async () => await UpdateFee(args.SignalTime));

    private async Task UpdateFee(DateTime updateTime)
    {
        var gasPriceInWei = await _web3.Eth.GasPrice.SendRequestAsync();
        var gasPriceInEth = Web3.Convert.FromWei(gasPriceInWei);

        Fee = gasPriceInEth * GasLimitOfTransferTransaction;
        _lastUpdate = updateTime;
    }

    private async Task ExecuteConcurrently(Func<Task> action)
    {
        Interlocked.Increment(ref _synchronizer);

        while (_synchronizer != 1)
            Thread.Yield();

        await action();

        Interlocked.Decrement(ref _synchronizer);
    }

    public void Dispose()
    {
        _timer.Elapsed -= ElapsedEventHandler;
        _timer.Stop();

        Join();

        _timer.Dispose();
    }

    private void Join()
    {
        while (_synchronizer != 0)
            Thread.Yield();
    }
    
    public decimal Fee { get; private set; }

    public double TimeToUpdateInMs => (DateTime.Now - _lastUpdate).Milliseconds;

    private DateTime _lastUpdate;
    
    private int _synchronizer;

    private readonly Web3 _web3;
    
    private readonly Timer _timer;
    
    private const decimal GasLimitOfTransferTransaction = 21_000m;
}