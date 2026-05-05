# syntax=docker/dockerfile:1.7

ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

COPY purchase-transactions.slnx ./
COPY PurchaseTransactions.Api/PurchaseTransactions.Api.csproj PurchaseTransactions.Api/
COPY PurchaseTransactions.Tests/PurchaseTransactions.Tests.csproj PurchaseTransactions.Tests/
RUN dotnet restore PurchaseTransactions.Api/PurchaseTransactions.Api.csproj

COPY PurchaseTransactions.Api/ PurchaseTransactions.Api/
RUN dotnet publish PurchaseTransactions.Api/PurchaseTransactions.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=false

EXPOSE 8080

HEALTHCHECK --interval=5s --timeout=3s --start-period=10s --retries=10 \
    CMD curl -fsS http://localhost:8080/api/health || exit 1

USER app

COPY --from=build --chown=app:app /app/publish .

ENTRYPOINT ["dotnet", "PurchaseTransactions.Api.dll"]
