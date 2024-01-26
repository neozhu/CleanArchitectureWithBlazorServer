using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs;
using System.Text.Json;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using DocumentFormat.OpenXml.InkML;
using FluentEmail.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;

public class AccessTokenProvider : IAccessTokenProvider
{
    private readonly ICurrentUserService _currentUser;
    private readonly ProtectedLocalStorage _localStorage;
    private readonly ILoginService _loginService;
    private readonly string _refreshTokenKey = nameof(_refreshTokenKey);
    private readonly IRefreshTokenValidator _refreshTokenValidator;
    private readonly ITenantProvider _tenantProvider;
    private readonly IAccessTokenGenerator _tokenGenerator;
    private readonly string _tokenKey = nameof(_tokenKey);
    private readonly IAccessTokenValidator _tokenValidator;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    //private readonly UserManager<ApplicationUser> _userManager;
    private readonly CustomUserManager _userManager;

    private readonly IMapper _mapper;

    public AccessTokenProvider(IServiceScopeFactory scopeFactory,
        ProtectedLocalStorage localStorage,
        ILoginService loginService,
        IAccessTokenValidator tokenValidator,
        IRefreshTokenValidator refreshTokenValidator,
        IAccessTokenGenerator tokenGenerator,
        ITenantProvider tenantProvider,
        ICurrentUserService currentUser, IMapper mapper)
    {
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<CustomUserManager>();
        _userClaimsPrincipalFactory =
            scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _localStorage = localStorage;
        _loginService = loginService;
        _tokenValidator = tokenValidator;
        _refreshTokenValidator = refreshTokenValidator;
        _tokenGenerator = tokenGenerator;
        _tenantProvider = tenantProvider;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }

    public async Task<string?> Login(ApplicationUser applicationUser)
    {
        var principal = await _userClaimsPrincipalFactory.CreateAsync(applicationUser);


        //extra in nammadhu
        //_tenantProvider.TenantId = applicationUser.TenantId;
        //_tenantProvider.TenantName = applicationUser.TenantName;
        //_currentUser.UserId = applicationUser.Id;
        //_currentUser.UserName = applicationUser.UserName;
        //_currentUser.TenantId = applicationUser.TenantId;
        //_currentUser.TenantName = applicationUser.TenantName;
        //_currentUser.UserRoleTenants ??= new List<ApplicationUserRoleTenantDto>();

        SetUserPropertiesFromClaimsPrincipal(principal);
        if (_currentUser.UserRoleTenants != null && _currentUser.UserRoleTenants.Count==0)
            applicationUser.UserRoleTenants.ForEach(x => _currentUser.UserRoleTenants.Add(_mapper.Map<ApplicationUserRoleTenantDto>(x)));


        var token = await _loginService.LoginAsync(principal);
        await _localStorage.SetAsync(_tokenKey, token);
        AccessToken = token.AccessToken;
        RefreshToken = token.RefreshToken;
        return AccessToken;
    }

    public async Task<ClaimsPrincipal> ParseClaimsFromJwt(string? accessToken)
    {
        if (string.IsNullOrEmpty(accessToken)) return new ClaimsPrincipal(new ClaimsIdentity());
        var validationResult = await _tokenValidator.ValidateTokenAsync(accessToken);
        if (validationResult.IsValid)
            return SetUserPropertiesFromClaimsPrincipal(new ClaimsPrincipal(validationResult.ClaimsIdentity));
        return new ClaimsPrincipal(new ClaimsIdentity());
    }

    public async Task<ClaimsPrincipal> GetClaimsPrincipal()
    {
        try
        {
            var token = await _localStorage.GetAsync<AuthenticatedUserResponse>(_tokenKey);
            if (token.Success && token.Value is not null)
            {
                AccessToken = token.Value.AccessToken;
                RefreshToken = token.Value.RefreshToken;
                var validationResult = await _tokenValidator.ValidateTokenAsync(AccessToken!);
                if (validationResult.IsValid)
                    return SetUserPropertiesFromClaimsPrincipal(new ClaimsPrincipal(validationResult.ClaimsIdentity));

                var validationRefreshResult = await _refreshTokenValidator.ValidateTokenAsync(RefreshToken!);
                if (validationRefreshResult.IsValid)
                    return SetUserPropertiesFromClaimsPrincipal(
                        new ClaimsPrincipal(validationRefreshResult.ClaimsIdentity));
            }
        }
        catch (CryptographicException)
        {
            await RemoveAuthDataFromStorage();
        }
        catch (Exception)
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        return new ClaimsPrincipal(new ClaimsIdentity());
    }


    public ValueTask RemoveAuthDataFromStorage()
    {
        return _localStorage.DeleteAsync(_tokenKey);
    }

    private ClaimsPrincipal SetUserPropertiesFromClaimsPrincipal(ClaimsPrincipal principal)
    {
        _tenantProvider.TenantId = principal.GetTenantId();
        _tenantProvider.TenantName = principal.GetTenantName();
        _currentUser.UserId = principal.GetUserId();
        _currentUser.UserName = principal.GetUserName();
        _currentUser.TenantId = principal.GetTenantId();
        _currentUser.TenantName = principal.GetTenantName(); // This seems to be an error in original code. Fixing it here.
        _currentUser.UserRoleTenants = principal.GetUserRoleTenants();
        return principal;
    }

    public async Task<string> Refresh(string refreshToken)
    {
        var validationResult = await _tokenValidator.ValidateTokenAsync(refreshToken!);
        if (!validationResult.IsValid) throw validationResult.Exception;
        var jwt = validationResult.SecurityToken as JwtSecurityToken;
        var userId = jwt!.Claims.First(claim => claim.Type == "id").Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new Exception($"no found user by userId:{userId}");
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var accessToken = _tokenGenerator.GenerateAccessToken(principal);
        return accessToken;
    }
}