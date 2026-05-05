using PurchaseTransactions.Api.Domain.Exceptions;

namespace PurchaseTransactions.Api.Domain.Entities;

public class PurchaseTransaction
{
  private PurchaseTransaction() { } // EF Core requires a parameterless constructor for materialization
  private PurchaseTransaction(string description, decimal amountUsd, DateTime date)
  {
    Id = Guid.NewGuid();
    CreatedAt = DateTime.UtcNow;
    Date = date;
    Description = description;
    AmountUsd = amountUsd;
  }

  public Guid Id { get; private set; }

  public string Description { get; private set; } = null!;

  public decimal AmountUsd { get; private set; }

  public DateTime Date { get; private set; }

  public DateTime CreatedAt { get; private set; }

  public DateTime UpdatedAt { get; private set; }

  public static PurchaseTransaction Create(string description, decimal amountUsd, DateTime date)
  {
    if (string.IsNullOrWhiteSpace(description))
      throw new DomainException("Description cannot be null or empty.");
    if (amountUsd <= 0)
      throw new DomainException("Amount must be a positive number.");
    if (date > DateTime.UtcNow)
      throw new DomainException("Date cannot be in the future.");

    return new PurchaseTransaction(description, amountUsd, date);
  }
}
