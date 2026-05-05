using PurchaseTransactions.Api.Application.Abstractions;
using PurchaseTransactions.Api.Domain.Entities;

namespace PurchaseTransactions.Api.Infrastructure.Persistence.Repositories;

public class PurchaseTransactionRepository(AppDbContext db) : IPurchaseTransactionRepository
{
  private readonly AppDbContext _db = db;

  public async Task AddAsync(PurchaseTransaction purchaseTransaction, CancellationToken cancellationToken)
  {
    _db.PurchaseTransactions.Add(purchaseTransaction);
    await _db.SaveChangesAsync(cancellationToken);
  }

  public Task<PurchaseTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => _db.PurchaseTransactions.FindAsync([id], cancellationToken).AsTask();
}
