// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Infrastructure.Constants.LocalStorage;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    public CurrentUserService(
        ProtectedLocalStorage protectedLocalStorage
       )
    {
        _protectedLocalStorage = protectedLocalStorage;
    }

    public async Task<string> UserId()
    {
        try
        {
            var userId = string.Empty;
            var storedPrincipal = await _protectedLocalStorage.GetAsync<string>(LocalStorage.USERID);
            if (storedPrincipal.Success && storedPrincipal.Value is not null)
            {
                userId = storedPrincipal.Value;
            }

            return userId;
        }
        catch
        {
            return String.Empty;
        }
    }
    public async Task<string> UserName()
    {
        try
        {
            var userName = string.Empty;
            var storedPrincipal = await _protectedLocalStorage.GetAsync<string>(LocalStorage.USERNAME);
            if (storedPrincipal.Success && storedPrincipal.Value is not null)
            {
                userName = storedPrincipal.Value;
            }

            return userName;
        }
        catch
        {
            return String.Empty;
        }
    }
}
