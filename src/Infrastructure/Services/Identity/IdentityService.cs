// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Configurations;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly AppConfigurationSettings _appConfig;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IAppCache _cache;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<IdentityService> _localizer;
    private TimeSpan RefreshInterval => TimeSpan.FromSeconds(60);
    private LazyCacheEntryOptions Options => new LazyCacheEntryOptions().SetAbsoluteExpiration(RefreshInterval, ExpirationMode.LazyExpiration);
    public IdentityService(
        IServiceScopeFactory scopeFactory,
        AppConfigurationSettings appConfig,
        IAppCache cache,
         IMapper mapper,
        IStringLocalizer<IdentityService> localizer)
    {
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        _userClaimsPrincipalFactory = scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _authorizationService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        _appConfig = appConfig;
        _cache = cache;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<string?> GetUserNameAsync(string userId, CancellationToken cancellation = default)
    {
        var key = $"GetUserNameAsync:{userId}";
        var user = await _cache.GetOrAddAsync(key, async () => await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId), Options);
        return user?.UserName;
    }
    public string GetUserName(string userId)
    {
        var key = $"GetUserName-byId:{userId}";
        var user = _cache.GetOrAdd(key, () => _userManager.Users.SingleOrDefault(u => u.Id == userId), Options);
        return user?.UserName??string.Empty;
    }
    public async Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ?? throw new NotFoundException(_localizer["User Not Found."]);
        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ?? throw new NotFoundException(_localizer["User Not Found."]);
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = await _authorizationService.AuthorizeAsync(principal, policyName);
        return result.Succeeded;

    }

    public async Task<Result> DeleteUserAsync(string userId, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ?? throw new NotFoundException(_localizer["User Not Found."]);
        var result = await _userManager.DeleteAsync(user);
        return result.ToApplicationResult();
    }
    public async Task<IDictionary<string, string?>> FetchUsers(string roleName, CancellationToken cancellation = default)
    {
        var result = await _userManager.Users
             .Where(x => x.UserRoles.Any(y => y.Role.Name == roleName))
             .Include(x => x.UserRoles)
             .ToDictionaryAsync(x => x.UserName!, y => y.DisplayName, cancellation);
        return result;
    }

    public async Task<Result<TokenResponse>> LoginAsync(TokenRequest request, CancellationToken cancellation = default)
    {
        var user = await _userManager.FindByNameAsync(request.UserName!);
        if (user == null)
        {
            return await Result<TokenResponse>.FailureAsync(new string[] { _localizer["User Not Found."] });
        }
        if (!user.IsActive)
        {
            return await Result<TokenResponse>.FailureAsync(new string[] { _localizer["User Not Active. Please contact the administrator."] });
        }
        if (!user.EmailConfirmed)
        {
            return await Result<TokenResponse>.FailureAsync(new string[] { _localizer["E-Mail not confirmed."] });
        }
        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password!);
        if (!passwordValid)
        {
            return await Result<TokenResponse>.FailureAsync(new string[] { _localizer["Invalid Credentials."] });
        }
        user.RefreshToken = GenerateRefreshToken();
        var tokenExpiryTime = DateTime.Now.AddDays(7);

        if (request.RememberMe)
        {
            tokenExpiryTime = DateTime.Now.AddYears(1);
        }
        user.RefreshTokenExpiryTime = tokenExpiryTime;
        await _userManager.UpdateAsync(user);
        
        var token = await GenerateJwtAsync(user);
        var response = new TokenResponse { Token = token, RefreshTokenExpiryTime = tokenExpiryTime, RefreshToken = user.RefreshToken, ProfilePictureDataUrl = user.ProfilePictureDataUrl };
        return await Result<TokenResponse>.SuccessAsync(response);
    }

    public async Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellation = default)
    {
        if (request is null)
        {
            return await Result<TokenResponse>.FailureAsync(new string[] { _localizer["Invalid Client Token."] });
        }
        var userPrincipal = GetPrincipalFromExpiredToken(request.Token);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email)!;
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
            return await Result<TokenResponse>.FailureAsync(new string[] { _localizer["User Not Found."] });
        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            return await Result<TokenResponse>.FailureAsync(new string[] { _localizer["Invalid Client Token."] });
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var token = GenerateEncryptedToken(GetSigningCredentials(), principal.Claims);
        user.RefreshToken = GenerateRefreshToken();
        await _userManager.UpdateAsync(user);

        var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
        return await Result<TokenResponse>.SuccessAsync(response);
    }
    public async Task<ClaimsPrincipal> GetClaimsPrincipal(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Secret)),
            ValidateIssuer =false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var result =await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);
        if (result.IsValid)
        {
           return new ClaimsPrincipal(result.ClaimsIdentity);
        }
        return new ClaimsPrincipal(new ClaimsIdentity());
    }
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    public async Task<string> GenerateJwtAsync(ApplicationUser user)
    {
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var token = GenerateEncryptedToken(GetSigningCredentials(), principal.Claims);
        return token;
    }
    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
           claims: claims,
           expires: DateTime.UtcNow.AddDays(2),
           signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);
        return encryptedToken;
    }
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException(_localizer["Invalid token"]);
        }
        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_appConfig.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    public async Task UpdateLiveStatus(string userId, bool isLive, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId && x.IsLive != isLive);
        if (user is not null)
        {
            user.IsLive = isLive;
            var result= await _userManager.UpdateAsync(user);
        }
    }
    public async Task<ApplicationUserDto> GetApplicationUserDto(string userId, CancellationToken cancellation = default)
    {
        var key = $"GetApplicationUserDto:{userId}";
        var result = await _cache.GetOrAddAsync(key, async () => await _userManager.Users.Where(x => x.Id == userId).Include(x => x.UserRoles).ThenInclude(x => x.Role).ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).FirstAsync(cancellation), Options);
        return result;
    }
    public async Task<List<ApplicationUserDto>?> GetUsers(string tenantId, CancellationToken cancellation = default)
    {
        var key = $"GetApplicationUserDtoListWithTenantId:{tenantId}";
        var result = await _cache.GetOrAddAsync(key, async () => await _userManager.Users.Where(x => x.TenantId == tenantId).Include(x => x.UserRoles).ThenInclude(x => x.Role)
                      .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).ToListAsync(), Options);
        return result;
    }
}
