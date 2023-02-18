// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Exceptions;

public class ForbiddenException : ServerException
{
    public ForbiddenException(string message) : base(message,System.Net.HttpStatusCode.Forbidden) { }
}
