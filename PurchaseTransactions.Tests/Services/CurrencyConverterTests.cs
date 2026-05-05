using FluentAssertions;
using NSubstitute;
using PurchaseTransactions.Api.Application.Abstractions;
using PurchaseTransactions.Api.Application.DTOs;
using PurchaseTransactions.Api.Application.Services;
using PurchaseTransactions.Api.Domain.Entities;
using PurchaseTransactions.Api.Domain.ValueObjects;

namespace PurchaseTransactions.Tests.Services;

public class CurrencyConverterTests
{
  private readonly ITreasuryRatesClient _rates = Substitute.For<ITreasuryRatesClient>();
  private readonly CurrencyConverter _sut;

  public CurrencyConverterTests() => _sut = new CurrencyConverter(_rates);

  private static PurchaseTransaction Tx(decimal amount = 100m, DateTime? date = null) => PurchaseTransaction.Create(
    "lunch",
    amount,
    // add date -3 months by default to avoid "older than 6 months" error in tests
    date ?? DateTime.UtcNow.AddMonths(-3)
  );

  [Fact]
  public async Task Convert_WhenRateExistsOnDate_ReturnsConverted()
  {
    var tx = Tx(100m, DateTime.UtcNow.AddMonths(-1));
    _rates.GetLatestRateAsync("Brazil-Real", tx.Date, Arg.Any<CancellationToken>())
          .Returns(new ExchangeRate("Brazil-Real", tx.Date, 5.00m));

    var result = await _sut.ConvertAsync(tx, "Brazil-Real", default);

    result.Ok.Should().BeTrue();
    result.Value!.ExchangeRate.Should().Be(5.00m);
    result.Value.ConvertedAmount.Should().Be(500.00m);
    result.Value.AmountUsd.Should().Be(100m);
    result.Value.Id.Should().Be(tx.Id);
  }

  [Fact]
  public async Task Convert_UsesOlderRate_WhenWithinSixMonths()
  {
    var tx = Tx(date: DateTime.UtcNow.AddMonths(-5));
    _rates.GetLatestRateAsync("X", tx.Date, Arg.Any<CancellationToken>())
          .Returns(new ExchangeRate("X", DateTime.UtcNow.AddMonths(-5), 2m));// ~5mo old

    var result = await _sut.ConvertAsync(tx, "X", default);

    result.Ok.Should().BeTrue();
    result.Value!.ExchangeRate.Should().Be(2m);
  }

  [Fact]
  public async Task Convert_WhenRateOlderThanSixMonths_ReturnsError()
  {
    var tx = Tx(date: new DateTime(2025, 6, 15));
    _rates.GetLatestRateAsync("X", tx.Date, Arg.Any<CancellationToken>())
          .Returns(new ExchangeRate("X", new DateTime(2024, 12, 14), 2m)); // >6mo

    var result = await _sut.ConvertAsync(tx, "X", default);

    result.Ok.Should().BeFalse();
    result.Error.Should().Contain($"Transaction date {tx.Date:yyyy-MM-dd} is older than 6 months");
  }

  [Fact]
  public async Task Convert_WhenNoRateReturned_ReturnsError()
  {
    var tx = Tx();
    _rates.GetLatestRateAsync(default!, default, default)
          .ReturnsForAnyArgs((ExchangeRate?)null);

    var result = await _sut.ConvertAsync(tx, "Nowhere", default);

    result.Ok.Should().BeFalse();
    result.Error.Should().Contain($"No exchange rate found for Nowhere on {tx.Date:yyyy-MM-dd}");
  }

  [Theory]
  [InlineData(10.00, 0.123456, 1.23)]   // 1.23456 → 1.23
  [InlineData(10.00, 0.125000, 1.25)]
  [InlineData(10.00, 0.126500, 1.27)]   // 1.265 → 1.27 (away from zero)
  [InlineData(33.33, 3.14159, 104.71)]
  public async Task Convert_RoundsToTwoDecimals_AwayFromZero(decimal usd, decimal rate, decimal expected)
  {
    var tx = Tx(usd);
    _rates.GetLatestRateAsync(default!, default, default)
          .ReturnsForAnyArgs(new ExchangeRate("X", tx.Date, rate));

    var result = await _sut.ConvertAsync(tx, "X", default);

    result.Value!.ConvertedAmount.Should().Be(expected);
  }

  [Fact]
  public async Task Convert_PassesPurchaseDateAsUpperBound()
  {
    var tx = Tx(date: DateTime.UtcNow.AddMonths(-1));
    _rates.GetLatestRateAsync(default!, default, default)
          .ReturnsForAnyArgs(new ExchangeRate("X", tx.Date, 1m));

    await _sut.ConvertAsync(tx, "X", default);

    await _rates.Received(1).GetLatestRateAsync("X", tx.Date, Arg.Any<CancellationToken>());
  }
}
