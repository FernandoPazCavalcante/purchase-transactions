using PurchaseTransactions.Api.Application.Abstractions;
using PurchaseTransactions.Api.Application.DTOs;
using PurchaseTransactions.Api.Domain.Entities;

namespace PurchaseTransactions.Api.Application.Services;

public class CurrencyConverter(ITreasuryRatesClient rates) : ICurrencyConverter
{
  private readonly ITreasuryRatesClient _rates = rates;

  public async Task<ConversionResult> ConvertAsync(PurchaseTransaction tx, string currency, CancellationToken ct)
  {
    if (tx.Date < DateTime.UtcNow.AddMonths(-6))
      return new ConversionResult(false, null, $"Transaction date {tx.Date:yyyy-MM-dd} is older than 6 months");

    var rate = await _rates.GetLatestRateAsync(currency, tx.Date, ct);
    if (rate is null)
      return new ConversionResult(false, null, $"No exchange rate found for {currency} on {tx.Date:yyyy-MM-dd}");

    var converted = Math.Round(tx.AmountUsd * rate.Rate, 2, MidpointRounding.AwayFromZero);

    var convertedTransaction = new ConvertedTransactionDTO(
      tx.Id,
      string.Empty,
      DateTime.UtcNow,
      tx.AmountUsd,
      rate.Rate,
      converted
    );
    return new ConversionResult(true, convertedTransaction, null);
  }
}
