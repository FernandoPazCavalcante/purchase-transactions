namespace PurchaseTransactions.Api.Application.DTOs;

public record class ConvertedTransactionDTO(
  Guid Id,
  string Description,
  DateTime Date,
  decimal AmountUsd,
  decimal ExchangeRate,
  decimal ConvertedAmount
);
