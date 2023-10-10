using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public class DefaultAuthenticator : IAuthenticator
{
    private readonly CustomUserManager _userManager;
    public DefaultAuthenticator(IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<CustomUserManager>();
    }
    public async Task<ApplicationUser?> Authenticate(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return null;
        }

        var correctPassword = await _userManager.CheckPasswordAsync(user, password);

        if (!correctPassword)
        {
            return null;
        }

        return user;
    }
}
