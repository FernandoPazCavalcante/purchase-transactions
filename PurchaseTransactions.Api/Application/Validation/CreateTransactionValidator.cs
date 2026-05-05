using FluentValidation;
using PurchaseTransactions.Api.Application.DTOs;

namespace PurchaseTransactions.Api.Application.Validation;

public class CreateTransactionValidator : AbstractValidator<CreateTransactionDTO>
{
  public CreateTransactionValidator()
  {
    _ = RuleFor(x => x.Description).NotEmpty().MaximumLength(50);
    _ = RuleFor(x => x.Date).NotEmpty().LessThanOrEqualTo(_ => DateTime.UtcNow);
    _ = RuleFor(x => x.AmountUsd).GreaterThan(0).PrecisionScale(19, 2, false);
  }
}
