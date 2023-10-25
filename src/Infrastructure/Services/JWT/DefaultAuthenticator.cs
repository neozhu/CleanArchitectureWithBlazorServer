using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;
using CleanArchitecture.Blazor.Domain.Features.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public class DefaultAuthenticator : IAuthenticator
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAccessTokenValidator _tokenValidator;
    private readonly IAccessTokenGenerator _tokenGenerator;

    public DefaultAuthenticator(IServiceScopeFactory scopeFactory, IAccessTokenValidator tokenValidator, IAccessTokenGenerator tokenGenerator)
    {
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _tokenValidator = tokenValidator;
        _tokenGenerator = tokenGenerator;
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

    public async Task<string> RefreshToken(string refreshToken)
    {
        TokenValidationResult validationResult = await _tokenValidator.ValidateTokenAsync(refreshToken!);
        if (!validationResult.IsValid)
        {
            return string.Empty;
        }
        JwtSecurityToken? jwt = validationResult.SecurityToken as JwtSecurityToken;
        string userId = jwt!.Claims.First(claim => claim.Type == "id").Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return string.Empty;
        }
        string accessToken =await _tokenGenerator.GenerateAccessToken(user);
        return accessToken;
    }
}
