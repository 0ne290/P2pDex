namespace Core.Domain.Exceptions;

public class DevelopmentErrorException : Exception
{
    public DevelopmentErrorException(string message) : base(message) { }
}