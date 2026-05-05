namespace PurchaseTransactions.Api.Application.DTOs;

public record CreateTransactionDTO(string Description, DateTime Date, decimal AmountUsd);
