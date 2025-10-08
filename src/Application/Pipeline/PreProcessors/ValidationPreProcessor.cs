namespace CleanArchitecture.Blazor.Application.Pipeline.PreProcessors;

public sealed class ValidationPreProcessor<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly IReadOnlyCollection<IValidator<TRequest>> _validators;

    public ValidationPreProcessor(IEnumerable<IValidator<TRequest>> validators)
    {
        if (validators is null) throw new ArgumentNullException(nameof(validators));
        _validators = validators.ToList();
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return;

        var failures = await _validators.ValidateAsync(request, cancellationToken);

        if (failures.Any()) throw new ValidationException(failures);
    }
}
