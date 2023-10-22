namespace CleanArchitecture.Blazor.Application.Common.Extensions;
public static class ValidationHelper
{
    public static async Task<IDictionary<string, string[]>> ValidateAsync<TRequest>(IEnumerable<IValidator<TRequest>> validators, ValidationContext<TRequest> validationContext, CancellationToken cancellationToken = default)
    {
        if (validators.Any())
        {
            var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            return failures.GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(g => g.Key, g => g.ToArray());
        }

        return new Dictionary<string, string[]>();
    }
}
