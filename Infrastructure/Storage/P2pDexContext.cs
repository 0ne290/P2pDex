using Infrastructure.Storage.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Storage;

public class P2pDexContext : DbContext
{
    public P2pDexContext(DbContextOptions<P2pDexContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Trader>()
            .HasMany<Order>()
            .WithOne()
            .HasForeignKey(o => o.BuyerGuid)
            .IsRequired(false);

        modelBuilder.Entity<Trader>()
            .HasMany<Order>()
            .WithOne()
            .HasForeignKey(o => o.SellerGuid)
            .IsRequired(false);
    }

    public DbSet<Trader> Traders { get; set; } = null!;

    public DbSet<Order> Orders { get; set; } = null!;
}