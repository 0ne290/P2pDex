using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class P2PDexDbContext : DbContext
{
    public P2PDexDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SellOrder>().HasKey(o => o.Guid);
        modelBuilder.Entity<Trader>().HasKey(t => t.Guid);
        
        modelBuilder.Entity<Trader>()
            .HasMany<SellOrder>()
            .WithOne()
            .HasForeignKey(o => o.SellerGuid)
            .IsRequired();
        
        modelBuilder.Entity<Trader>()
            .HasMany<SellOrder>()
            .WithOne()
            .HasForeignKey(o => o.BuyerGuid)
            .IsRequired(false);
    }

    public DbSet<Trader> Traders { get; set; } = null!;

    public DbSet<SellOrder> SellOrders { get; set; } = null!;
}