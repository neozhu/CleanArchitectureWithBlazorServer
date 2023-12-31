// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs;

public class TokenResponse
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}