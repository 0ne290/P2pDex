namespace Core.Application.Interfaces;

public interface IFeeCalculator
{
    Task<(decimal FeeSellerToExchanger, decimal ExpectedFeeExchangerToMiners)> Calculate(decimal cryptoAmount);
}