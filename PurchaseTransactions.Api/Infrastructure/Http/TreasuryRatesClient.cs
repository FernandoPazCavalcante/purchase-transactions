using System.Globalization;
using System.Text.Json.Serialization;
using PurchaseTransactions.Api.Application.Abstractions;
using PurchaseTransactions.Api.Domain.ValueObjects;

namespace PurchaseTransactions.Api.Infrastructure.Http;

public class TreasuryRatesClient(HttpClient http) : ITreasuryRatesClient
{
  private const string PATH = "services/api/fiscal_service/v1/accounting/od/rates_of_exchange";
  private readonly HttpClient _http = http;

  public async Task<ExchangeRate?> GetLatestRateAsync(
      string currency, DateTime onOrBefore, CancellationToken ct)
  {
    var date = onOrBefore.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    var url = $"{PATH}" +
              $"?fields=country_currency_desc,exchange_rate,record_date" +
              $"&filter=country_currency_desc:eq:{Uri.EscapeDataString(currency)}," +
              $"record_date:lte:{date}" +
              $"&sort=-record_date" +
              $"&page[size]=1";

    using var res = await _http.GetAsync(url, ct);
    _ = res.EnsureSuccessStatusCode();

    var payload = await res.Content.ReadFromJsonAsync<RatesResponse>(cancellationToken: ct);
    var row = payload?.Data?.FirstOrDefault();
    if (row is null) return null;

    return new ExchangeRate(
        row.CountryCurrencyDesc,
        DateTime.Parse(row.RecordDate, CultureInfo.InvariantCulture),
        decimal.Parse(row.ExchangeRate, CultureInfo.InvariantCulture));
  }

  private sealed record RatesResponse(
      [property: JsonPropertyName("data")] List<RateRow>? Data);

  private sealed record RateRow(
      [property: JsonPropertyName("country_currency_desc")] string CountryCurrencyDesc,
      [property: JsonPropertyName("exchange_rate")] string ExchangeRate,
      [property: JsonPropertyName("record_date")] string RecordDate);
}
