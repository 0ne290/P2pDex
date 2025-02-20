using Core.Application.Api.SellOrder.Get;
using Core.Application.Private.Interfaces;
using Infrastructure.Persistence.Private;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Public;

public class SellOrderAndItsTradersQuery : ISellOrderAndItsTradersQuery
{
    public SellOrderAndItsTradersQuery(P2PDexDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<SellOrderAndItsTradersDto?> Execute(Guid orderGuid)
        => await DbContext.Database
            .SqlQuery<SellOrderAndItsTradersDto>(
                $"""
                 SELECT SellOrders.Status, SellOrders.SellerId, Sellers.Name, SellOrders.BuyerId, Buyers.Name,
                        SellOrders.Crypto, SellOrders.CryptoAmount, SellOrders.Fiat, SellOrders.CryptoToFiatExchangeRate,
                        SellOrders.FiatAmount, SellOrders.PaymentMethodInfo
                 WHERE SellOrders.Guid = {orderGuid.ToString()}
                 FROM SellOrders
                     INNER JOIN Traders Sellers ON SellOrders.SellerId = Sellers.Id
                     LEFT JOIN Traders Buyers ON SellOrders.BuyerId = Buyers.Id;
                 """
                ).FirstOrDefaultAsync();
    
    public readonly P2PDexDbContext DbContext;
}