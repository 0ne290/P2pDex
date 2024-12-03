using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Interfaces;
using Infrastructure.Storage.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Storage.Implementations;

public class OrderStorage : IOrderStorage
{
    public OrderStorage(P2pDexContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Add(OrderBase orderEntity)
    {
        await _dbContext.Orders.AddAsync(new Order(orderEntity));

        await _dbContext.SaveChangesAsync();
        
        _dbContext.ChangeTracker.Clear();
    }

    public async Task<OrderBase?> TryGetByGuid(string guid)
    {
        var orderModel = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Guid == guid);

        if (orderModel == null)
            return null;

        return await orderModel.ToEntity(_dbContext);
    }

    public async Task<ICollection<OrderBase>> GetAllByStatus(OrderStatus status)
    {
        var orderModels = await _dbContext.Orders.Where(o => o.Status == status).ToListAsync();
        var orderEntities = new List<OrderBase>(orderModels.Count);

        foreach (var orderModel in orderModels)
            orderEntities.Add(await orderModel.ToEntity(_dbContext));

        return orderEntities;
    }

    public async Task UpdateAll(IEnumerable<OrderBase> orderEntities)
    {
        var orderModels = orderEntities.Select(o => new Order(o)).ToList();
        
        _dbContext.Orders.AttachRange(orderModels);
        
        foreach (var orderModel in orderModels)
            _dbContext.Entry(orderModel).State = EntityState.Modified;

        await _dbContext.SaveChangesAsync();
        
        _dbContext.ChangeTracker.Clear();
    }

    public async Task Update(OrderBase orderEntity)
    {
        var orderModel = new Order(orderEntity);
        
        _dbContext.Orders.Attach(orderModel);
        
        _dbContext.Entry(orderModel).State = EntityState.Modified;

        await _dbContext.SaveChangesAsync();
        
        _dbContext.ChangeTracker.Clear();
    }

    private readonly P2pDexContext _dbContext;
}