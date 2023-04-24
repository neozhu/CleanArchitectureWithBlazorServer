// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class NotFoundException : ServerException
{


    public NotFoundException(string message)
        : base(message,System.Net.HttpStatusCode.NotFound)
    {
    }
   public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.", System.Net.HttpStatusCode.NotFound)
    {
    }
}
