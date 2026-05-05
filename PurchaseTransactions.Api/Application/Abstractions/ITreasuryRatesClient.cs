using PurchaseTransactions.Api.Domain.ValueObjects;

namespace PurchaseTransactions.Api.Application.Abstractions;

public interface ITreasuryRatesClient
{
  Task<ExchangeRate?> GetLatestRateAsync(string currency, DateTime onOrBefore, CancellationToken ct);
}
