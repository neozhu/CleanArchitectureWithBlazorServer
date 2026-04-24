namespace CleanArchitecture.Blazor.Application.Pipeline;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IMessage
{
    private readonly IReadOnlyCollection<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        if (validators is null) throw new ArgumentNullException(nameof(validators));
        _validators = validators.ToList();
    }

    public async ValueTask<TResponse> Handle(
        TRequest request,
        MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var failures = await _validators.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

            if (failures.Any())
                throw new ValidationException(string.Join(", ", failures.Select(x => x.ErrorMessage)));
        }

        return await next(request, cancellationToken).ConfigureAwait(false);
    }
}
