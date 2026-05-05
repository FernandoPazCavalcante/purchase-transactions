using PurchaseTransactions.Api.Application.DTOs;
using PurchaseTransactions.Api.Domain.Entities;

namespace PurchaseTransactions.Api.Application.Abstractions;

public interface ICurrencyConverter
{
  Task<ConversionResult> ConvertAsync(PurchaseTransaction tx, string currency, CancellationToken ct);
}
