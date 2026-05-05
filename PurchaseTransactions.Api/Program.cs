using Microsoft.EntityFrameworkCore;
using PurchaseTransactions.Api.Api;
using PurchaseTransactions.Api.Application;
using PurchaseTransactions.Api.Infrastructure;
using PurchaseTransactions.Api.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApiServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  db.Database.Migrate();
}

app.MapOpenApi();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Purchase Transactions API v1"));

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();
