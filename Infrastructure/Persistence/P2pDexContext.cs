using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class P2PDexContext : DbContext
{
    public P2PDexContext(DbContextOptions<P2PDexContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SellOrder>()
            .HasOne(o => o.Seller)
            .WithMany()
            .HasForeignKey("SellerGuid")
            .IsRequired();
        
        modelBuilder.Entity<SellOrder>()
            .HasOne(o => o.Buyer)
            .WithMany()
            .HasForeignKey("BuyerGuid")
            .IsRequired();
    }

    public DbSet<Trader> Traders { get; set; } = null!;

    public DbSet<SellOrder> SellOrders { get; set; } = null!;
}