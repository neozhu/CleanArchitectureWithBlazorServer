// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Domain.Common.Entities;

public interface ISoftDelete
{
    DateTime? DeletedAt { get; set; }
    string? DeletedById { get; set; }
}
