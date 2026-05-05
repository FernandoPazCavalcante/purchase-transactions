using FluentAssertions;
using PurchaseTransactions.Api.Infrastructure.Http;

namespace PurchaseTransactions.Tests.Integration;

[Trait("Category", "Live")]
public class TreasureRatesClientContractTests
{
  private static TreasuryRatesClient NewSut() =>
    new(new HttpClient
    {
      BaseAddress = new Uri("https://api.fiscaldata.treasury.gov/"),
      Timeout = TimeSpan.FromSeconds(15)
    });

  [Fact]
  public async Task GetLatestRate_ReturnsRate_ForKnownCurrency()
  {
    var sut = NewSut();
    var rate = await sut.GetLatestRateAsync("Brazil-Real", DateTime.UtcNow, default);

    rate.Should().NotBeNull();
    rate!.Currency.Should().Be("Brazil-Real");
    rate.Rate.Should().BeGreaterThan(0);
    rate.EffectiveDate.Should().BeBefore(DateTime.UtcNow.AddDays(1));
  }

  [Fact]
  public async Task GetLatestRate_RespectsOnOrBeforeDate()
  {
    var sut = NewSut();
    var cutoff = new DateTime(2020, 6, 30);

    var rate = await sut.GetLatestRateAsync("Brazil-Real", cutoff, default);

    rate.Should().NotBeNull();
    rate!.EffectiveDate.Should().BeOnOrBefore(cutoff);
  }

  [Fact]
  public async Task GetLatestRate_ReturnsNull_ForUnknownCurrency()
  {
    var sut = NewSut();
    var rate = await sut.GetLatestRateAsync("Mars-Credit", DateTime.UtcNow, default);
    rate.Should().BeNull();
  }
}
