namespace Core.Application.Interfaces;

public interface IFeeCalculator
{
    Task<decimal> Calculate(decimal cryptoAmount);
}