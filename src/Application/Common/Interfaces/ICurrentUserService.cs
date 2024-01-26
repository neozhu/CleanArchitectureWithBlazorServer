﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; set; }
    string? UserName { get; set; }
    ICollection<ApplicationUserRoleTenantDto>? UserRoles { get; set; } 
    string? TenantId { get; set; }
    string? TenantName { get; set; }
}