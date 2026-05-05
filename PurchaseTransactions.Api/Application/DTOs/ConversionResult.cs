namespace PurchaseTransactions.Api.Application.DTOs;

public record ConversionResult(bool Ok, ConvertedTransactionDTO? Value, string? Error);
