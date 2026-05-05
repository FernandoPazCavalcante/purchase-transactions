using Microsoft.AspNetCore.Mvc;
using PurchaseTransactions.Api.Api.Filters;
using PurchaseTransactions.Api.Application.Abstractions;
using PurchaseTransactions.Api.Application.DTOs;
using PurchaseTransactions.Api.Domain.Entities;

namespace PurchaseTransactions.Api.Api.Endpoints;

public static class TransactionsEndpoints
{
  public static IEndpointRouteBuilder MapTransactions(this IEndpointRouteBuilder app)
  {
    var api = app.MapGroup("/api/transactions");

    api.MapGet("/{id:guid}", async (
        Guid id,
        IPurchaseTransactionRepository repository,
        CancellationToken cancellationToken) =>
    {
      var transaction = await repository.GetByIdAsync(id, cancellationToken);
      return transaction is null ? Results.NotFound() : Results.Ok(transaction);
    })
    .WithName("GetTransaction");

    api.MapGet("/{id:guid}/convert", async (
        Guid id,
        [FromQuery] string currency,
        IPurchaseTransactionRepository repository,
        ICurrencyConverter converter,
        CancellationToken cancellationToken) =>
    {
      var transaction = await repository.GetByIdAsync(id, cancellationToken);
      if (transaction is null) return Results.NotFound();

      var result = await converter.ConvertAsync(transaction, currency, cancellationToken);
      return result.Ok
        ? Results.Ok(result.Value)
        : Results.UnprocessableEntity(new { error = result.Error });
    })
    .WithName("ConvertTransaction");

    api.MapPost("/", async (
        [FromBody] CreateTransactionDTO payload,
        IPurchaseTransactionRepository repository,
        CancellationToken cancellationToken) =>
    {
      var purchaseTransaction = PurchaseTransaction.Create(
          payload.Description,
          payload.AmountUsd,
          payload.Date
      );

      await repository.AddAsync(purchaseTransaction, cancellationToken);

      return Results.Created($"/api/transactions/{purchaseTransaction.Id}", purchaseTransaction);
    })
    .AddEndpointFilter<ValidationFilter<CreateTransactionDTO>>()
    .WithName("CreateTransaction");

    return app;
  }
}
