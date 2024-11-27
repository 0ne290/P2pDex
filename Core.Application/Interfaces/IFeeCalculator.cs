namespace Core.Application.Interfaces;

public interface IFeeCalculator
{
    Task<(decimal SellerToExchanger, decimal ExpectedExchangerToMiners)> Calculate(decimal cryptoAmount);
}