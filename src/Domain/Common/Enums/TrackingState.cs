// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Domain.Common.Enums;

public enum TrackingState
{
    /// <summary>Existing entity that has not been modified.</summary>
    Unchanged,

    /// <summary>Newly created entity.</summary>
    Added,

    /// <summary>Existing entity that has been modified.</summary>
    Modified,

    /// <summary>Existing entity that has been marked as deleted.</summary>
    Deleted
}