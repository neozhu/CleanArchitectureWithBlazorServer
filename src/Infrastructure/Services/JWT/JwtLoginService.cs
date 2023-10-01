using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
/// <summary>
/// Use this class to log a user in.
/// </summary>
/// <typeparam name="TUser"></typeparam>
public class JwtLoginService : ILoginService
{
    private readonly ITokenGeneratorService tokenGenerator;

    public JwtLoginService(ITokenGeneratorService tokenGenerator)
    {
        this.tokenGenerator = tokenGenerator;
    }

    /// <summary>
    /// Use this method to get an access Token and a refresh Token for the given TUser
    /// </summary>
    /// <param name="user"></param>
    /// <returns>An instance of <see cref="AuthenticatedUserResponse"/>, containing an access Token and a refresh Token</returns>
    public async Task<AuthenticatedUserResponse> LoginAsync(ApplicationUser user)
    {
        var accessToken =await tokenGenerator.GenerateAccessToken(user);
        var refreshToken = await tokenGenerator.GenerateRefreshToken(user);
        
        return new AuthenticatedUserResponse()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }
}

