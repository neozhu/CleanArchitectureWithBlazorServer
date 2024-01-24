// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs;

public class TokenRequest
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public bool RememberMe { get; set; }
}