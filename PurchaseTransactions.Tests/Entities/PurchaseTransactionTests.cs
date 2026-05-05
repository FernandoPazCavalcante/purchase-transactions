using FluentAssertions;
using PurchaseTransactions.Api.Domain.Entities;
using PurchaseTransactions.Api.Domain.Exceptions;

namespace PurchaseTransactions.Tests.Entities;

public class PurchaseTransactionTests
{
  [Fact]
  public void Create_ShouldThrowDomainException_WhenDescriptionIsNullOrEmpty()
  {
    // Arrange
    var description = string.Empty;
    var amountUsd = 100m;
    var date = DateTime.UtcNow;

    // Act
    Action act = () => PurchaseTransaction.Create(description, amountUsd, date);

    // Assert
    act.Should().Throw<DomainException>();
  }

  [Fact]
  public void Create_ShouldThrowDomainException_WhenAmountIsNotAPositiveNumber()
  {
    // Given
    var description = "Test Description";
    var amountUsd = -100m;
    var date = DateTime.UtcNow;

    // Act
    Action act = () => PurchaseTransaction.Create(description, amountUsd, date);

    // Assert
    act.Should().Throw<DomainException>();
  }

  [Fact]
  public void Create_ShouldThrowDomainException_WhenDateGreaterThanUtcNow()
  {
    // Given
    var description = "Test Description";
    var amountUsd = 100m;
    var date = DateTime.UtcNow.AddDays(1);

    // Act
    Action act = () => PurchaseTransaction.Create(description, amountUsd, date);

    // Assert
    act.Should().Throw<DomainException>();
  }

}
