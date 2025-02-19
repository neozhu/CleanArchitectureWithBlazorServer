namespace CleanArchitecture.Blazor.Application.Common.Models;

/// <summary>
/// Represents the result of an operation.
/// </summary>
public class Result : IResult
{
    /// <summary>
    /// Protected constructor to initialize the result.
    /// </summary>
    /// <param name="succeeded">Indicates whether the operation succeeded.</param>
    /// <param name="errors">A collection of error messages.</param>
    protected Result(bool succeeded, IEnumerable<string>? errors)
    {
        Succeeded = succeeded;
        Errors = errors?.ToArray() ?? Array.Empty<string>();
    }

    /// <summary>
    /// Indicates whether the operation succeeded.
    /// </summary>
    public bool Succeeded { get; init; }

    /// <summary>
    /// An array of error messages.
    /// </summary>
    public string[] Errors { get; init; }

    /// <summary>
    /// Gets a concatenated string of error messages, separated by commas.
    /// </summary>
    public string ErrorMessage => string.Join(", ", Errors);

    /// <summary>
    /// Creates a successful <see cref="Result"/> instance.
    /// </summary>
    public static Result Success() => new Result(true, Array.Empty<string>());

    /// <summary>
    /// Asynchronously creates a successful <see cref="Result"/> instance.
    /// </summary>
    public static Task<Result> SuccessAsync() => Task.FromResult(Success());

    /// <summary>
    /// Creates a failed <see cref="Result"/> instance.
    /// </summary>
    /// <param name="errors">Error messages.</param>
    public static Result Failure(params string[] errors) => new Result(false, errors);

    /// <summary>
    /// Asynchronously creates a failed <see cref="Result"/> instance.
    /// </summary>
    /// <param name="errors">Error messages.</param>
    public static Task<Result> FailureAsync(params string[] errors) => Task.FromResult(Failure(errors));

    /// <summary>
    /// Executes the appropriate function based on whether the operation succeeded.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="onSuccess">Function to execute if the operation succeeded.</param>
    /// <param name="onFailure">Function to execute if the operation failed, with an error message.</param>
    public TResult Match<TResult>(Func<TResult> onSuccess, Func<string, TResult> onFailure)
        => Succeeded ? onSuccess() : onFailure(ErrorMessage);

    /// <summary>
    /// Asynchronously executes the appropriate function based on whether the operation succeeded.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="onSuccess">Asynchronous function to execute if the operation succeeded.</param>
    /// <param name="onFailure">Asynchronous function to execute if the operation failed, with an error message.</param>
    public Task<TResult> MatchAsync<TResult>(Func<Task<TResult>> onSuccess, Func<string, Task<TResult>> onFailure)
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
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Protected constructor to initialize a result with data.
    /// </summary>
    /// <param name="succeeded">Indicates whether the operation succeeded.</param>
    /// <param name="errors">A collection of error messages.</param>
    /// <param name="data">The data returned from the operation.</param>
    protected Result(bool succeeded, IEnumerable<string>? errors, T? data)
        : base(succeeded, errors)
    {
        Data = data;
    }

    /// <summary>
    /// Creates a successful <see cref="Result{T}"/> instance with the specified data.
    /// </summary>
    /// <param name="data">The data to include in the result.</param>
    public static Result<T> Success(T data) => new Result<T>(true, Array.Empty<string>(), data);

    /// <summary>
    /// Asynchronously creates a successful <see cref="Result{T}"/> instance with the specified data.
    /// </summary>
    /// <param name="data">The data to include in the result.</param>
    public static Task<Result<T>> SuccessAsync(T data) => Task.FromResult(Success(data));

    /// <summary>
    /// Creates a failed <see cref="Result{T}"/> instance.
    /// </summary>
    /// <param name="errors">Error messages.</param>
    public static new Result<T> Failure(params string[] errors) => new Result<T>(false, errors, default);

    /// <summary>
    /// Asynchronously creates a failed <see cref="Result{T}"/> instance.
    /// </summary>
    /// <param name="errors">Error messages.</param>
    public static new Task<Result<T>> FailureAsync(params string[] errors) => Task.FromResult(Failure(errors));

    /// <summary>
    /// Executes the appropriate function based on whether the operation succeeded.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="onSuccess">Function to execute if the operation succeeded, with the data.</param>
    /// <param name="onFailure">Function to execute if the operation failed, with an error message.</param>
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure)
        => Succeeded ? onSuccess(Data!) : onFailure(ErrorMessage);

    /// <summary>
    /// Asynchronously executes the appropriate function based on whether the operation succeeded.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="onSuccess">Asynchronous function to execute if the operation succeeded, with the data.</param>
    /// <param name="onFailure">Asynchronous function to execute if the operation failed, with an error message.</param>
    public Task<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> onSuccess, Func<string, Task<TResult>> onFailure)
        => Succeeded ? onSuccess(Data!) : onFailure(ErrorMessage);

    /// <summary>
    /// Maps the data contained in the result to a new type.
    /// </summary>
    /// <typeparam name="TResult">The type of the mapped data.</typeparam>
    /// <param name="map">The mapping function.</param>
    public Result<TResult> Map<TResult>(Func<T, TResult> map)
        => Succeeded ? Result<TResult>.Success(map(Data!)) : Result<TResult>.Failure(Errors);

    /// <summary>
    /// Asynchronously maps the data contained in the result to a new type.
    /// </summary>
    /// <typeparam name="TResult">The type of the mapped data.</typeparam>
    /// <param name="map">The asynchronous mapping function.</param>
    public async Task<Result<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> map)
        => Succeeded ? Result<TResult>.Success(await map(Data!)) : await Result<TResult>.FailureAsync(Errors);

    /// <summary>
    /// Binds the result to another result, allowing for chaining operations.
    /// </summary>
    /// <typeparam name="TResult">The type of the data in the resulting result.</typeparam>
    /// <param name="bind">The binding function that returns a new result.</param>
    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> bind)
        => Succeeded ? bind(Data!) : Result<TResult>.Failure(Errors);

    /// <summary>
    /// Asynchronously binds the result to another result, allowing for chaining operations.
    /// </summary>
    /// <typeparam name="TResult">The type of the data in the resulting result.</typeparam>
    /// <param name="bind">The asynchronous binding function that returns a new result.</param>
    public async Task<Result<TResult>> BindAsync<TResult>(Func<T, Task<Result<TResult>>> bind)
        => Succeeded ? await bind(Data!) : await Result<TResult>.FailureAsync(Errors);
}
