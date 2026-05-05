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

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=false

EXPOSE 8080

USER app

COPY --from=build --chown=app:app /app/publish .

ENTRYPOINT ["dotnet", "PurchaseTransactions.Api.dll"]
