namespace Core.Domain.Constants;

public static class FiatCurrency
{
    public static bool IsFiatCurrency(string value) => value == Ruble;

    public const string Ruble = "Ruble";
}