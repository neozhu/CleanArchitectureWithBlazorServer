// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{

    private readonly AuthenticationStateProvider _stateProvider;

    //public CurrentUserService(
    //    AuthenticationStateProvider stateProvider
       
    //   )
    //{
    //    _stateProvider = stateProvider;
    //}
    public async Task<string?> UserId()
    {
        var state = await _stateProvider.GetAuthenticationStateAsync();
        return state.User?.GetUserId();
    }

    public async Task<string?> UserName()
    {
        var state = await _stateProvider.GetAuthenticationStateAsync();
        return state.User?.Identity?.Name;
    }
}
