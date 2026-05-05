using FluentValidation;
using PurchaseTransactions.Api.Application.Abstractions;
using PurchaseTransactions.Api.Application.Services;
using PurchaseTransactions.Api.Application.Validation;

namespace PurchaseTransactions.Api.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    _ = services.AddValidatorsFromAssemblyContaining<CreateTransactionValidator>();
    _ = services.AddScoped<ICurrencyConverter, CurrencyConverter>();
    return services;
  }
}
