using FluentValidation.Results;

namespace CleanArchitecture.Blazor.Application.Common.Extensions;

public static class ValidationExtensions
{
    public static async Task<List<ValidationFailure>> ValidateAsync<TRequest>(
        this IEnumerable<IValidator<TRequest>> validators, ValidationContext<TRequest> validationContext,
        CancellationToken cancellationToken = default)
    {
        if (!validators.Any()) return new List<ValidationFailure>();

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)));

        return validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();
    }

    public static Dictionary<string, string[]> ToDictionary(this List<ValidationFailure>? failures)
    {
        return failures != null && failures.Any()
            ? failures.GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.ToArray())
            : new Dictionary<string, string[]>();
    }
}