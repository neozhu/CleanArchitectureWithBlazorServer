// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

/// <summary>
/// Represents the result of an operation.
/// </summary>
public interface IResult
{
    IReadOnlyList<string> Errors { get; init; }

    bool Succeeded { get; init; }
}
/// <summary>
/// Represents the result of an operation that includes data.
/// </summary>
/// <typeparam name="T">The type of the data.</typeparam>
public interface IResult<out T> : IResult
{
    T? Data { get; }
}
