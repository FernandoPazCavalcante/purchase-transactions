using PurchaseTransactions.Api.Api.Endpoints;

namespace PurchaseTransactions.Api.Api;

public static class DependencyInjection
{
  public static IServiceCollection AddApiServices(this IServiceCollection services)
  {
    _ = services.AddOpenApi();
    _ = services.AddHealthChecks();
    return services;
  }

  public static WebApplication MapEndpoints(this WebApplication app)
  {
    _ = app.MapHealthChecks("/api/health").WithName("HealthCheck");
    _ = app.MapTransactions();
    return app;
  }
}
