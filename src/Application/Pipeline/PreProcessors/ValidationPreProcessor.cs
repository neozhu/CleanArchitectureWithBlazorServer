namespace CleanArchitecture.Blazor.Application.Pipeline.PreProcessors;

public sealed class ValidationPreProcessor<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly IReadOnlyCollection<IValidator<TRequest>> _validators;

    public ValidationPreProcessor(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators.ToList() ?? throw new ArgumentNullException(nameof(validators));
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return;

        var validationContext = new ValidationContext<TRequest>(request);

        var failures = await _validators.ValidateAsync(validationContext, cancellationToken);

        if (failures.Any()) throw new ValidationException(failures);
    }
}