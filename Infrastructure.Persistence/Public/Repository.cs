using System.Collections;
using System.Linq.Expressions;
using Core.Application.Private.Interfaces;
using Core.Domain.Constants;
using Infrastructure.Persistence.Private;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Public;

public class Repository : IRepository
{
    public Repository(P2PDexDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task Add<TEntity>(TEntity entity) where TEntity : class =>
        await DbContext.Set<TEntity>().AddAsync(entity);

    public void UpdateRange<TEntity>(IEnumerable<TEntity> updatedEntities) where TEntity : class =>
        DbContext.Set<TEntity>().UpdateRange(updatedEntities);

    public async Task<bool> Exists<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class =>
        await DbContext.Set<TEntity>().AnyAsync(filter);

    public async Task<TEntity?> TryGet<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class =>
        await DbContext.Set<TEntity>().FirstOrDefaultAsync(filter);

    public async Task<ICollection<TEntity>> GetAllBy<TEntity>(Expression<Func<TEntity, bool>> filter)
        where TEntity : class => await DbContext.Set<TEntity>().AsNoTracking().Where(filter).ToListAsync();

    public async Task<ICollection> GetAllSellOrdersBySellers() => await DbContext.SellOrders.AsNoTracking()
        .Where(o => o.Status == OrderStatus.SellerToExchangerTransferTransactionConfirmed).Join(DbContext.Traders,
            o => o.SellerId, t => t.Id,
            (o, t) => new
            {
                sellerId = t.Id, sellerName = t.Name, guid = o.Guid, crypto = o.Crypto, cryptoAmount = o.CryptoAmount,
                fiat = o.Fiat, cryptoToFiatExchangeRate = o.CryptoToFiatExchangeRate, fiatAmount = o.FiatAmount,
                paymentMethodInfo = o.PaymentMethodInfo
            }).ToListAsync();

    public readonly P2PDexDbContext DbContext;
}