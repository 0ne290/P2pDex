using Core.Application.Api.SellOrder.Get;
using Core.Application.Private.Interfaces;
using Infrastructure.Persistence.Private;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Public;

public class SellOrderAndItsTradersQuery : ISellOrderAndItsTradersQuery
{
    public SellOrderAndItsTradersQuery(P2PDexDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SellOrderAndItsTradersDto?> Execute(Guid orderGuid)
    {
        FormattableString query = $"""
                                   SELECT SellOrders.Status, SellOrders.SellerId, Sellers.Name AS SellerName, SellOrders.BuyerId,
                                          Buyers.Name AS BuyerName, SellOrders.Crypto, SellOrders.CryptoAmount, SellOrders.Fiat,
                                          SellOrders.CryptoToFiatExchangeRate, SellOrders.FiatAmount, SellOrders.PaymentMethodInfo
                                   FROM SellOrders
                                       INNER JOIN Traders Sellers ON SellOrders.SellerId = Sellers.Id
                                       LEFT JOIN Traders Buyers ON SellOrders.BuyerId = Buyers.Id
                                   WHERE SellOrders.Guid = {orderGuid.ToString().ToUpper()}
                                   """;

        return await _dbContext.Database.SqlQuery<SellOrderAndItsTradersDto>(query).FirstOrDefaultAsync();
    }


    private readonly P2PDexDbContext _dbContext;
}