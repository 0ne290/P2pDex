namespace Core.Domain.Constants;

public static class Cryptocurrency
{
    public static bool IsCryptocurrency(string value) => value == Ethereum;
    
    public const string Ethereum = "Ethereum";
}