namespace Core.Application.Commands;

public class ConfirmBySellerOfCryptocurrencyTransferTransactionCommand
{
    public required string SellerGuid { get; init; }
    
    public required string OrderGuid { get; init; }
    
    public required string TransactionHash { get; init; }
}