namespace Core.Domain.Interfaces;

public interface IUnitOfWork
{
    Task Save();
    
    IRepository Repository { get; }
}