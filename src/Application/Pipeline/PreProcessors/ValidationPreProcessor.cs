namespace CleanArchitecture.Blazor.Application.Pipeline.PreProcessors;

public sealed class ValidationPreProcessor<TRequest, TResponse> : MessagePreProcessor<TRequest, TResponse>
    where TRequest : notnull, IMessage
{
    private readonly IReadOnlyCollection<IValidator<TRequest>> _validators;

    public ValidationPreProcessor(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators.ToList() ?? throw new ArgumentNullException(nameof(validators));
    }

    protected override async ValueTask Handle(TRequest request, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return;

        var validationContext = new ValidationContext<TRequest>(request);

        var failures = await _validators.ValidateAsync(validationContext, cancellationToken);

        if (failures.Any())
            throw new ValidationException(failures);
    }
}
