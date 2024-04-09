// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User.GetUserId();
    public string? UserName => _httpContextAccessor.HttpContext?.User.GetUserName();
    public string? TenantId => _httpContextAccessor.HttpContext?.User.GetTenantId();
    public string? TenantName => _httpContextAccessor.HttpContext?.User.GetTenantName();
}