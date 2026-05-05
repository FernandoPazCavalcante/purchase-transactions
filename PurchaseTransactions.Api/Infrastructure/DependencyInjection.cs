using Microsoft.EntityFrameworkCore;
using PurchaseTransactions.Api.Application.Abstractions;
using PurchaseTransactions.Api.Infrastructure.Http;
using PurchaseTransactions.Api.Infrastructure.Persistence;
using PurchaseTransactions.Api.Infrastructure.Persistence.Repositories;

namespace PurchaseTransactions.Api.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(
            configuration.GetConnectionString("Default"),
            npgsql => npgsql.EnableRetryOnFailure(maxRetryCount: 5)
        ));

    services.AddScoped<IPurchaseTransactionRepository, PurchaseTransactionRepository>();

    services.AddHttpClient<ITreasuryRatesClient, TreasuryRatesClient>(c =>
    {
      c.BaseAddress = new Uri("https://api.fiscaldata.treasury.gov/");
      c.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddStandardResilienceHandler(o =>
    {
      o.Retry.MaxRetryAttempts = 3;
      o.Retry.Delay = TimeSpan.FromMilliseconds(500);
      o.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
      o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);
      o.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(20);
    });

    return services;
  }
}
