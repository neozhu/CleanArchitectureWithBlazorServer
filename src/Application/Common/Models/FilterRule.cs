// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Models;

public class FilterRule
{
    public string? Field { get; set; }
    public string? Op { get; set; }
    public string? Value { get; set; }
}
