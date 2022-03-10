// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ProtectedLocalStorage _protectedLocalStorage;



    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        ProtectedLocalStorage protectedLocalStorage
       )
    {
        _httpContextAccessor = httpContextAccessor;
        _protectedLocalStorage = protectedLocalStorage;


    }

    public async Task<string> UserId()
    {
        try
        {
            var userId = "";
            if (_httpContextAccessor.HttpContext != null)
            {
                userId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            if (string.IsNullOrEmpty(userId))
            {
                var storedPrincipal = await _protectedLocalStorage.GetAsync<string>("UserId");
                if (storedPrincipal.Success && storedPrincipal.Value is not null)
                {
                    userId = storedPrincipal.Value;
                }
            }
            return userId??String.Empty;
        }
        catch
        {
            return String.Empty;
        }
    }
}
