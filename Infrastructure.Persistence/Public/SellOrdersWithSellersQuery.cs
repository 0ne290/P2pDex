using System.Linq.Expressions;
using Core.Application.Api.SellOrder.Commands;
using Core.Application.Private.Interfaces;
using Core.Domain.Entities;
using Infrastructure.Persistence.Private;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Public;

public class SellOrdersWithSellersQuery : ISellOrdersWithSellersQuery
{
    public SellOrdersWithSellersQuery(P2PDexDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ICollection<SellOrderWithSeller>> Execute(Expression<Func<SellOrder, bool>> filter) => await _dbContext.SellOrders
        .AsNoTracking().Where(filter).Join(_dbContext.Traders, o => o.SellerId, t => t.Id,
            (o, t) => new SellOrderWithSeller
            {
                SellerId = t.Id, SellerName = t.Name, Guid = o.Guid, Crypto = o.Crypto, CryptoAmount = o.CryptoAmount,
                Fiat = o.Fiat, CryptoToFiatExchangeRate = o.CryptoToFiatExchangeRate, FiatAmount = o.FiatAmount,
                PaymentMethodInfo = o.PaymentMethodInfo
            }).ToListAsync();

    private readonly P2PDexDbContext _dbContext;
}