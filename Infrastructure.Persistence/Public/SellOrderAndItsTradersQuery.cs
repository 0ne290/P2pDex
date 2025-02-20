using System.Linq.Expressions;
using Core.Application.Api.SellOrder.Get;
using Core.Application.Private.Interfaces;
using Core.Domain.Entities;
using Infrastructure.Persistence.Private;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Public;

public class SellOrderAndItsTradersQuery : ISellOrderAndItsTradersQuery
{
    public SellOrderAndItsTradersQuery(P2PDexDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<SellOrderAndItsTradersDto?> Execute(Expression<Func<SellOrder, bool>> filter) => await DbContext
        .SellOrders
        .AsNoTracking().Where(filter)
        .Join(DbContext.Traders, o => o.SellerId, t => t.Id,
            (o, t) => new
            {
                status = o.Status, sellerId = t.Id, sellerName = t.Name, buyerId = o.BuyerId, crypto = o.Crypto,
                cryptoAmount = o.CryptoAmount, fiat = o.Fiat, cryptoToFiatExchangeRate = o.CryptoToFiatExchangeRate,
                fiatAmount = o.FiatAmount, paymentMethodInfo = o.PaymentMethodInfo
            }).Join(DbContext.Traders, o => o.buyerId, t => t.Id,
            (o, t) => new SellOrderAndItsTradersDto
            {
                Status = o.status, SellerId = o.sellerId, SellerName = o.sellerName, BuyerId = o.buyerId,
                BuyerName = t.Name, Crypto = o.crypto, CryptoAmount = o.cryptoAmount, Fiat = o.fiat,
                CryptoToFiatExchangeRate = o.cryptoToFiatExchangeRate, FiatAmount = o.fiatAmount,
                PaymentMethodInfo = o.paymentMethodInfo
            }).DefaultIfEmpty().FirstOrDefaultAsync();
    
    public readonly P2PDexDbContext DbContext;
}