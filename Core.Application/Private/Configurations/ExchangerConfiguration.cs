namespace Core.Application.Private.Configurations;

public class ExchangerConfiguration
{
    public ExchangerConfiguration(decimal feeRate, string accountAddress)
    {
        FeeRate = feeRate;
        AccountAddress = accountAddress;
    }

    public readonly decimal FeeRate;

    public readonly string AccountAddress;
}