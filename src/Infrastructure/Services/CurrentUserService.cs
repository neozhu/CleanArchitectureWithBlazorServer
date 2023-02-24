// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor
       )
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public string? UserId => _httpContextAccessor.HttpContext?.User.GetUserId();
    public string? Email => _httpContextAccessor.HttpContext?.User.GetEmail();
    public string? UserName => _httpContextAccessor.HttpContext?.User.GetUserName();
    public string? TenantId => _httpContextAccessor.HttpContext?.User.GetTenantId();
    public string? TenantName => _httpContextAccessor.HttpContext?.User.GetTenantName();
    public string? DisplayName => _httpContextAccessor.HttpContext?.User.GetDisplayName();
    public string? SuperiorId => _httpContextAccessor.HttpContext?.User.GetSuperiorId();
    public string? SuperiorName => _httpContextAccessor.HttpContext?.User.GetSuperiorName();
    public string? ProfilePictureDataUrl => _httpContextAccessor.HttpContext?.User.GetProfilePictureDataUrl();
    public string[]? AssignRoles
    {
        get
        {
            var str = _httpContextAccessor.HttpContext?.User.GetAssignRoles() ?? string.Empty;
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return str.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
