// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Authentication.Cookies;

namespace CleanArchitecture.Blazor.Infrastructure.Services;
#nullable disable
public class ConfigureCookieAuthenticationOptions
    : IPostConfigureOptions<CookieAuthenticationOptions>
{
    private readonly ITicketStore _ticketStore;

    public ConfigureCookieAuthenticationOptions(ITicketStore ticketStore)
    {
        _ticketStore = ticketStore;
    }

    public void PostConfigure(string name,
        CookieAuthenticationOptions options)
    {
        options.SessionStore = _ticketStore;
    }
}