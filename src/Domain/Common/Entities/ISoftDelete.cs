// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Domain.Common.Entities;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }
    string? DeletedBy { get; set; }
}