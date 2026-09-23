using FluentValidation;
using MediatR;

namespace Application.Common.Behaviors;

public class ValidateBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
: IPipelineBehavior<TRequest, TResponse>
where TRequest : notnull
{
      public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
      {
            if (validators.Any())
            {
                  var context = new ValidationContext<TRequest>(request);

                  var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                  var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                  if (failures.Count != 0)
                  {
                        // Domain exception (not FluentValidation's) so the API maps it to a 400 with a field -> messages map.
                        var errors = failures
                              .GroupBy(f => string.IsNullOrWhiteSpace(f.PropertyName) ? "request" : f.PropertyName)
                              .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).Distinct().ToArray());

                        throw new Domain.Exceptions.ValidationException(errors);
                  }
            }

            return await next();
      }
}

