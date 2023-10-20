using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Common.Behaviours;
public sealed class ValidationPreProcessor<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly IReadOnlyCollection<IValidator<TRequest>> _validators;

    public ValidationPreProcessor(IEnumerable<IValidator<TRequest>> validators) =>
        _validators = validators.ToList() ?? throw new ArgumentNullException(nameof(validators));

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return;

        var validationContext = new ValidationContext<TRequest>(request);
        var failures = await Task
            .WhenAll(_validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)))
            .ConfigureAwait(false);

        var validationFailures =
            failures.SelectMany(result => result.Errors).Where(failure => failure != null).ToList();

        if (validationFailures.Any()) throw new ValidationException(validationFailures);
    }
}
