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

    api.MapGet("/{id:guid}", (Guid id) => Results.Ok())
       .WithName("GetTransaction");

    api.MapGet("/{id:guid}/convert", (Guid id, [FromQuery] string currency) => Results.Ok())
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
