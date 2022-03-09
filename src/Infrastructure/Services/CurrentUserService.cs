// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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
            var storedPrincipal = await _protectedLocalStorage.GetAsync<string>("UserId");
            if (storedPrincipal.Success && storedPrincipal.Value is not null)
            {
                return storedPrincipal.Value;
            }
            return "";
        }
        catch
        {
            return "";
        }
    }
}
