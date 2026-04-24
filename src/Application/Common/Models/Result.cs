namespace CleanArchitecture.Blazor.Application.Common.Models;

/// <summary>
/// Represents the result of an operation.
/// </summary>
public class Result : IResult
{
    /// <summary>
    /// Protected constructor to initialize the result.
    /// </summary>
    /// <param name="succeeded">Indicates if the operation succeeded.</param>
    /// <param name="errors">A collection of error messages.</param>
    protected Result(bool succeeded, IEnumerable<string>? errors)
    {
        Succeeded = succeeded;
        // Convert to a read-only list to ensure immutability.
        Errors = errors?.ToList().AsReadOnly() ?? ((IReadOnlyList<string>)Array.Empty<string>());
    }

    /// <inheritdoc/>
    public bool Succeeded { get; init; }

    /// <inheritdoc/>
    public IReadOnlyList<string> Errors { get; init; }

    /// <summary>
    /// Gets a concatenated string of error messages separated by commas.
    /// </summary>
    public string ErrorMessage => string.Join(", ", Errors);

    /// <summary>
    /// Creates a successful <see cref="Result"/> instance.
    /// </summary>
    public static Result Success() => new(true, Array.Empty<string>());

    /// <summary>
    /// Asynchronously creates a successful <see cref="Result"/> instance.
    /// </summary>
    public static Task<Result> SuccessAsync() => Task.FromResult(Success());

    /// <summary>
    /// Creates a failed <see cref="Result"/> instance with a collection of error messages.
    /// </summary>
    /// <param name="errors">A collection of error messages.</param>
    public static Result Failure(params IEnumerable<string> errors) => new(false, errors);

    /// <summary>
    /// Asynchronously creates a failed <see cref="Result"/> instance with a collection of error messages.
    /// </summary>
    /// <param name="errors">A collection of error messages.</param>
    public static Task<Result> FailureAsync(params IEnumerable<string> errors) => Task.FromResult(Failure(errors));

    /// <summary>
    /// Executes the corresponding action based on whether the operation succeeded.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <param name="onFailure">Action to execute on failure (receives the error message).</param>
    public void Match(Action onSuccess, Action<string> onFailure)
    {
        if (Succeeded)
            onSuccess();
        else
            onFailure(ErrorMessage);
    }

    /// <summary>
    /// Asynchronously executes the corresponding action based on whether the operation succeeded.
    /// </summary>
    /// <param name="onSuccess">Async action to execute on success.</param>
    /// <param name="onFailure">Async action to execute on failure (receives the error message).</param>
    public Task MatchAsync(Func<Task> onSuccess, Func<string, Task> onFailure)
        => Succeeded ? onSuccess() : onFailure(ErrorMessage);
}

/// <summary>
/// Represents the result of an operation that includes data.
/// </summary>
/// <typeparam name="T">The type of the data.</typeparam>
public class Result<T> : Result, IResult<T>
{
    /// <summary>
    /// The data contained in the result.
    /// Note: For successful operations, it is recommended that Data is not null.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Protected constructor to initialize a result with data.
    /// </summary>
    /// <param name="succeeded">Indicates if the operation succeeded.</param>
    /// <param name="errors">A collection of error messages.</param>
    /// <param name="data">The data returned by the operation.</param>
    protected Result(bool succeeded, IEnumerable<string>? errors, T? data)
        : base(succeeded, errors)
    {
        Data = data;
    }

    /// <summary>
    /// Creates a successful <see cref="Result{T}"/> instance with data.
    /// </summary>
    /// <param name="data">The data to include in the result.</param>
    public static Result<T> Success(T data) => new(true, Array.Empty<string>(), data);

    /// <summary>
    /// Asynchronously creates a successful <see cref="Result{T}"/> instance with data.
    /// </summary>
    /// <param name="data">The data to include in the result.</param>
    public static Task<Result<T>> SuccessAsync(T data) => Task.FromResult(Success(data));
    /// <summary>
    /// Creates a failed <see cref="Result{T}"/> instance with a collection of error messages.
    /// </summary>
    /// <param name="errors">A collection of error messages.</param>
    public static new Result<T> Failure(params IEnumerable<string> errors) => new(false, errors, default);
    /// <summary>
    /// Asynchronously creates a failed <see cref="Result{T}"/> instance with a collection of error messages.
    /// </summary>
    /// <param name="errors">A collection of error messages.</param>
    public static new Task<Result<T>> FailureAsync(params IEnumerable<string> errors) => Task.FromResult(Failure(errors));

    /// <summary>
    /// Executes the corresponding action based on whether the operation succeeded.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success (receives the data).</param>
    /// <param name="onFailure">Action to execute on failure (receives the error message).</param>
    public void Match(Action<T> onSuccess, Action<string> onFailure)
    {
        if (Succeeded)
            onSuccess(Data!);
        else
            onFailure(ErrorMessage);
    }

    /// <summary>
    /// Asynchronously executes the corresponding action based on whether the operation succeeded.
    /// </summary>
    /// <param name="onSuccess">Async action to execute on success (receives the data).</param>
    /// <param name="onFailure">Async action to execute on failure (receives the error message).</param>
    public Task MatchAsync(Func<T, Task> onSuccess, Func<string, Task> onFailure)
        => Succeeded ? onSuccess(Data!) : onFailure(ErrorMessage);

    /// <summary>
    /// Maps the data contained in the result to a new type.
    /// </summary>
    /// <typeparam name="TResult">The new data type.</typeparam>
    /// <param name="map">Mapping function.</param>
    public Result<TResult> Map<TResult>(Func<T, TResult> map)
        => Succeeded ? Result<TResult>.Success(map(Data!)) : Result<TResult>.Failure(Errors);

    /// <summary>
    /// Asynchronously maps the data contained in the result to a new type.
    /// </summary>
    /// <typeparam name="TResult">The new data type.</typeparam>
    /// <param name="map">Async mapping function.</param>
    public async Task<Result<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> map)
        => Succeeded ? Result<TResult>.Success(await map(Data!)) : await Result<TResult>.FailureAsync(Errors);

    /// <summary>
    /// Binds the result to another result, enabling chained operations.
    /// </summary>
    /// <typeparam name="TResult">The data type of the resulting result.</typeparam>
    /// <param name="bind">Binding function that returns a new result.</param>
    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> bind)
        => Succeeded ? bind(Data!) : Result<TResult>.Failure(Errors);

    /// <summary>
    /// Asynchronously binds the result to another result, enabling chained operations.
    /// </summary>
    /// <typeparam name="TResult">The data type of the resulting result.</typeparam>
    /// <param name="bind">Async binding function that returns a new result.</param>
    public Task<Result<TResult>> BindAsync<TResult>(Func<T, Task<Result<TResult>>> bind)
        => Succeeded ? bind(Data!) : Result<TResult>.FailureAsync(Errors);
}
