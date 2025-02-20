using System.Collections;
using System.Linq.Expressions;
using Core.Application.Private.Interfaces;
using Core.Domain.Entities;
using Infrastructure.Persistence.Private;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Public;

public class SellOrdersAndTheirSellersQuery : ISellOrdersAndTheirSellersQuery
{
    public SellOrdersAndTheirSellersQuery(P2PDexDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<ICollection> Execute(Expression<Func<SellOrder, bool>> filter) => await DbContext.SellOrders
        .AsNoTracking().Where(filter).Join(DbContext.Traders, o => o.SellerId, t => t.Id,
            (o, t) => new
            {
                sellerId = t.Id, sellerName = t.Name, guid = o.Guid, crypto = o.Crypto, cryptoAmount = o.CryptoAmount,
                fiat = o.Fiat, cryptoToFiatExchangeRate = o.CryptoToFiatExchangeRate, fiatAmount = o.FiatAmount,
                paymentMethodInfo = o.PaymentMethodInfo
            }).ToListAsync();
    
    public readonly P2PDexDbContext DbContext;
}