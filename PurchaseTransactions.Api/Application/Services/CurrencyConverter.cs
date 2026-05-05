using PurchaseTransactions.Api.Application.Abstractions;
using PurchaseTransactions.Api.Application.DTOs;
using PurchaseTransactions.Api.Domain.Entities;

namespace PurchaseTransactions.Api.Application.Services;

public class CurrencyConverter(ITreasuryRatesClient rates) : ICurrencyConverter
{
  private readonly ITreasuryRatesClient _rates = rates;

  public async Task<ConversionResult> ConvertAsync(PurchaseTransaction tx, string currency, CancellationToken ct)
  {
    var rate = await _rates.GetLatestRateAsync(currency, tx.Date, ct);

    var windowStart = tx.Date.AddMonths(-6);
    if (rate is null || rate.EffectiveDate < windowStart)
      return new ConversionResult(false, null, $"the purchase cannot be converted to {currency}");

    var converted = Math.Round(tx.AmountUsd * rate.Rate, 2, MidpointRounding.AwayFromZero);

    var convertedTransaction = new ConvertedTransactionDTO(
      tx.Id,
      tx.Description,
      tx.Date,
      tx.AmountUsd,
      rate.Rate,
      converted
    );
    return new ConversionResult(true, convertedTransaction, null);
  }
}
