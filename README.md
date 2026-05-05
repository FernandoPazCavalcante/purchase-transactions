# Purchase Transactions API

REST API to record purchase transactions and convert their amounts to foreign
currencies using the U.S. Treasury Reporting Rates of Exchange.

## Tech stack

- .NET 10 (Minimal API)
- PostgreSQL 17 + EF Core 10
- FluentValidation, Polly (resilience), xUnit + FluentAssertions + NSubstitute
- Docker / docker-compose

## Project layout

Single project organized in layered folders (Clean Architecture lite):

```
PurchaseTransactions.Api/
  Domain/          entities, value objects, domain exceptions
  Application/     abstractions, services, DTOs, validation
  Infrastructure/  EF Core, repositories, HTTP clients
  Api/             endpoints, filters, DI composition
  Program.cs
PurchaseTransactions.Tests/
  Entities/        domain unit tests
  Services/        application unit tests (NSubstitute)
  Integration/     live contract tests (Treasury API)
```

## Prerequisites

- .NET 10 SDK
- Docker + docker-compose

## Run with Docker (recommended)

```bash
docker-compose up --build
```

API: http://localhost:8080
Swagger: http://localhost:8080/swagger
Postgres: localhost:5432 (db `purchase_transactions`, user `app`, password `app`)

## Run locally

Start Postgres only:

```bash
docker-compose up -d db
```

Run the API:

```bash
dotnet run --project PurchaseTransactions.Api
```

EF Core applies migrations on startup. The connection string lives in
`PurchaseTransactions.Api/appsettings.Development.json`.

## Endpoints

| Method | Route                                              | Description                          |
| ------ | -------------------------------------------------- | ------------------------------------ |
| POST   | `/api/transactions`                                | Create a purchase transaction        |
| GET    | `/api/transactions/{id}`                           | Retrieve a transaction               |
| GET    | `/api/transactions/{id}/convert?currency={name}`   | Retrieve converted to target currency |
| GET    | `/api/health`                                      | Health check                         |

`currency` uses the Treasury `country_currency_desc` format, e.g. `Brazil-Real`.

### Example

```bash
curl -X POST http://localhost:8080/api/transactions \
  -H 'Content-Type: application/json' \
  -d '{"description":"lunch","date":"2026-04-01","amountUsd":12.50}'
```

## Tests

Run unit tests (default, skips live API calls):

```bash
dotnet test --filter "Category!=Live"
```

Run live contract tests against the Treasury API (requires internet):

```bash
dotnet test --filter "Category=Live"
```

## Lint / format

The repo uses `.editorconfig` with .NET analyzers.

```bash
dotnet format purchase-transactions.slnx --verify-no-changes --severity warn
```

## CI

GitHub Actions runs build, lint, and tests on push and pull requests targeting
`main`. See `.github/workflows/ci.yml`.

---

## AI usage disclosure

I used an AI assistant to help produce a plan for the tests, a plan to reorganize my initial code into the layered architecture I had decided to adopt, and to draft this README itself. The implementation, decisions, and final code are my own.
