// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public class UserDto 
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required  string UserName { get; set; }  
    public string? DisplayName { get; set; }
    public string? Provider { get; set; } = "Local";
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsLive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public required string Email { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? PhoneNumber { get; set; }
    public string? SuperiorId { get; set; } 
    public string? SuperiorName { get; set; }
    public string[]? AssignRoles { get; set; }
    public bool Checked { get; set; }
    public string? Role { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }



}
