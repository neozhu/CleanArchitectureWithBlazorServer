// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Infrastructure.Configurations;

public class Theme
{
    public string ThemeVersion { get; set; } = default!;
    public string Logo { get; set; } = default!;
    public string User { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Twitter { get; set; } = default!;
    public string Avatar { get; set; } = default!;
}
