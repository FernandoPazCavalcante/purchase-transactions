namespace PurchaseTransactions.Api.Domain.ValueObjects;

public record class ExchangeRate(string Currency, DateTime EffectiveDate, decimal Rate);
