// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Application.Common.Interfaces;

public interface IResult
{
    string[] Errors { get; set; }

    bool Succeeded { get; set; }
}
public interface IResult<out T> : IResult
{
    T Data { get; }
}
