using Nethereum.Web3;

namespace Infrastructure;

public class ConcurrentWeb3Wrapper
{
    public ConcurrentWeb3Wrapper(Web3 web3)
    {
        _web3 = web3;
        _synchronizer = 0;
    }
    
    public async Task<T> Execute<T>(Func<Web3, Task<T>> handler)
    {
        Interlocked.Increment(ref _synchronizer);
        
        while (_synchronizer != 1)
            Thread.Yield();
        
        var ret = await handler(_web3);
        
        Interlocked.Decrement(ref _synchronizer);

        return ret;
    }
    
    private int _synchronizer;

    private readonly Web3 _web3;
}