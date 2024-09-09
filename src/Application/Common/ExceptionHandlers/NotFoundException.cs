// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;

namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class NotFoundException : ServerException
{
    public NotFoundException(string message)
        : base(message, HttpStatusCode.NotFound)
    {
    }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" with key ({key}) was not found.", HttpStatusCode.NotFound)
    {
    }
}