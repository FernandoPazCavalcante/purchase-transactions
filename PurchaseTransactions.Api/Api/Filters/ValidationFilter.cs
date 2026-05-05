using FluentValidation;

namespace PurchaseTransactions.Api.Api.Filters;

public class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
    where T : class
{
  private readonly IValidator<T> _validator = validator;

  public async ValueTask<object?> InvokeAsync(
      EndpointFilterInvocationContext context,
      EndpointFilterDelegate next
  )
  {
    var argument = context.Arguments.OfType<T>().FirstOrDefault();

    if (argument is null)
    {
      return Results.Problem(
          detail: $"Expected argument of type {typeof(T).Name} was not found.",
          statusCode: StatusCodes.Status400BadRequest
      );
    }

    var result = await _validator.ValidateAsync(argument, context.HttpContext.RequestAborted);

    if (!result.IsValid)
    {
      return Results.ValidationProblem(result.ToDictionary());
    }

    return await next(context);
  }
}
