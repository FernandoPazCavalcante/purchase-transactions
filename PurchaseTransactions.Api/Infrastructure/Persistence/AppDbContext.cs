using Microsoft.EntityFrameworkCore;
using PurchaseTransactions.Api.Domain.Entities;

namespace PurchaseTransactions.Api.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<PurchaseTransaction> PurchaseTransactions => Set<PurchaseTransaction>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<PurchaseTransaction>(entity =>
    {
      entity.ToTable("purchase_transactions");
      entity.HasKey(x => x.Id);
      entity.Property(x => x.Description).IsRequired().HasMaxLength(50);
      entity.Property(x => x.AmountUsd).HasPrecision(19, 2);
      entity.Property(x => x.CreatedAt);
      entity.Property(x => x.UpdatedAt);
    });
  }
}
