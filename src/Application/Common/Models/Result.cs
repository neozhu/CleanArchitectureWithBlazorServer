// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Models;

public class Result : IResult
{
    internal Result()
    {
        Errors = new string[] { };
    }

    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    public string ErrorMessage => string.Join(", ", Errors ?? new string[] { });

    public bool Succeeded { get; init; }

    public string[] Errors { get; init; }

    public static Result Success() => new Result(true, Array.Empty<string>());
    public static Task<Result> SuccessAsync() => Task.FromResult(Success());
    public static  Result Failure(params string[] errors) => new Result(false, errors);
    public static Task<Result> FailureAsync(params string[] errors) => Task.FromResult(Failure(errors));

    // Functional extensions
    public TResult Match<TResult>(Func<TResult> onSuccess, Func<string, TResult> onFailure) =>
        Succeeded ? onSuccess() : onFailure(ErrorMessage);

    public async Task<TResult> MatchAsync<TResult>(Func<Task<TResult>> onSuccess, Func<string, Task<TResult>> onFailure) =>
        Succeeded ? await onSuccess() : await onFailure(ErrorMessage);
}

public class Result<T> : Result, IResult<T>
{
    public T? Data { get; set; }
    public static Result<T> Success(T data) => new Result<T> { Succeeded= true, Data= data };
    public static Task<Result<T>> SuccessAsync(T data) => Task.FromResult(Success(data));
    public static new Result<T> Failure(params string[] errors) => new Result<T> { Succeeded = false, Errors = errors.ToArray() };
    public static new Task<Result<T>> FailureAsync(params string[] errors) => Task.FromResult(new Result<T> { Succeeded = false, Errors = errors.ToArray() });

    // Functional extensions
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure) =>
        Succeeded ? onSuccess(Data!) : onFailure(ErrorMessage);

    public async Task<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> onSuccess, Func<string, Task<TResult>> onFailure) =>
        Succeeded ? await onSuccess(Data!) : await onFailure(ErrorMessage);

    public Result<TResult> Map<TResult>(Func<T, TResult> map) =>
        Succeeded ? Result<TResult>.Success(map(Data!)) : Result<TResult>.Failure(Errors);

    public async Task<Result<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> map) =>
    Succeeded ? Result<TResult>.Success(await map(Data!)) : await Result<TResult>.FailureAsync(Errors);

    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> bind) =>
        Succeeded ? bind(Data!) : Result<TResult>.Failure(Errors);

    public async Task<Result<TResult>> BindAsync<TResult>(Func<T, Task<Result<TResult>>> bind) =>
    Succeeded ? await bind(Data!) : await Result<TResult>.FailureAsync(Errors);

}