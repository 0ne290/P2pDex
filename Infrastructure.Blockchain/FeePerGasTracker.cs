using System.Timers;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;
using Nethereum.Web3;
using Timer = System.Timers.Timer;

namespace Infrastructure.Blockchain;

public class FeePerGasTracker : IDisposable
{
    static FeePerGasTracker()
    {
        var maxPriorityInWei = Web3.Convert.ToWei(2, UnitConversion.EthUnit.Gwei);
        MaxPriorityValueInWei = maxPriorityInWei.ToHexBigInteger();
        MaxPriorityValueInEth = Web3.Convert.FromWei(maxPriorityInWei);
    }
    
    public FeePerGasTracker(Web3 web3, double intervalInMs)
    {
        _synchronizer = 0;

        _web3 = web3;

        _timer = new Timer { AutoReset = true, Enabled = false, Interval = intervalInMs };
        _timer.Elapsed += ElapsedEventHandler;
        
        var valueInWei = (_web3.Eth.GasPrice.SendRequestAsync().GetAwaiter().GetResult().Value * 2);
        Value = (valueInWei.ToHexBigInteger(), Web3.Convert.FromWei(valueInWei));
        _expectedNextUpdate = DateTime.Now + TimeSpan.FromMilliseconds(_timer.Interval);
        
        _timer.Start();
    }

    private async void ElapsedEventHandler(object? _, ElapsedEventArgs args) =>
        await ExecuteConcurrently(async () => await UpdateFee(args.SignalTime));

    private async Task UpdateFee(DateTime updateTime)
    {
        var baseInWei = (await _web3.Eth.GasPrice.SendRequestAsync()).Value * 2;
        var maxInWei = baseInWei + MaxPriorityValueInWei.Value;
        var maxInEth = Web3.Convert.FromWei(maxInWei);
        MaxValue = (maxInWei.ToHexBigInteger(), maxInEth);
        _expectedNextUpdate = updateTime + TimeSpan.FromMilliseconds(_timer.Interval);
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
    
    public (HexBigInteger InWei, decimal InEth) MaxValue { get; private set; }

    public static readonly HexBigInteger MaxPriorityValueInWei;
    
    public static readonly decimal MaxPriorityValueInEth;

    public double TimeToUpdateInMs
    {
        get
        {
            var ret = (_expectedNextUpdate - DateTime.Now).TotalMilliseconds;
            
            return ret < 0 ? 0 : ret;
        }
    }

    private DateTime _expectedNextUpdate;
    
    private int _synchronizer;

    private readonly Web3 _web3;
    
    private readonly Timer _timer;
}