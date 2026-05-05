using PurchaseTransactions.Api.Domain.Entities;

namespace PurchaseTransactions.Api.Application.Abstractions;

public interface IPurchaseTransactionRepository
{

  Task<PurchaseTransaction?> GetByIdAsync(Guid id, CancellationToken ct);
  Task AddAsync(PurchaseTransaction purchaseTransaction, CancellationToken ct);

}
