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

    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    public static Task<Result> SuccessAsync()
    {
        return Task.FromResult(new Result(true, Array.Empty<string>()));
    }

    public static Result Failure(params string[] errors)
    {
        return new Result(false, errors);
    }

    public static Task<Result> FailureAsync(params string[] errors)
    {
        return Task.FromResult(new Result(false, errors));
    }
}

public class Result<T> : Result, IResult<T>
{
    public T? Data { get; set; }

    public new static Result<T> Failure(params string[] errors)
    {
        return new Result<T> { Succeeded = false, Errors = errors.ToArray() };
    }

    public new static async Task<Result<T>> FailureAsync(params string[] errors)
    {
        return await Task.FromResult(Failure(errors));
    }

    public static Result<T> Success(T data)
    {
        return new Result<T> { Succeeded = true, Data = data };
    }

    public static async Task<Result<T>> SuccessAsync(T data)
    {
        return await Task.FromResult(Success(data));
    }
}