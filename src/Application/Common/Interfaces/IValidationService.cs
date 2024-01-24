using FluentValidation.Internal;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface IValidationService
{
    Func<object, string, Task<IEnumerable<string>>> ValidateValue<TRequest>();

    // NOTE: providing the model as parameter is not required,
    // it's just easier to write and nicer to read in the blazor component
    // instead of the explicit declaration of the model type
    Func<object, string, Task<IEnumerable<string>>> ValidateValue<TRequest>(TRequest _);

    Task<IDictionary<string, string[]>> ValidateAsync<TRequest>(TRequest model,
        CancellationToken cancellationToken = default);

    Task<IDictionary<string, string[]>> ValidateAsync<TRequest>(TRequest model,
        Action<ValidationStrategy<TRequest>> options, CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> ValidatePropertyAsync<TRequest>(TRequest model, string propertyName,
        CancellationToken cancellationToken = default);
}