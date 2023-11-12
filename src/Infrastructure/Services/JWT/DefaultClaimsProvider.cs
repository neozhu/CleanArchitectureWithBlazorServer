namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
/// <summary>
/// Use <see cref="DefaultClaimsProvider"/> to get access token claims and refresh token claims
/// </summary>
/// <typeparam name="TUserKey"></typeparam>
/// <typeparam name="TUser"></typeparam>
public class DefaultClaimsProvider : IClaimsProvider
{
    /// <summary>
    /// Provide claims for access token
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public virtual IEnumerable<Claim> ProvideAccessClaims(ClaimsPrincipal user)
    {
        return GetUserClaims(user);
    }

    /// <summary>
    /// Provide claims for refresh token
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public virtual IEnumerable<Claim> ProvideRefreshClaims(ClaimsPrincipal user)
    {
        return GetUserClaims(user);
    }

    private static IEnumerable<Claim> GetUserClaims(ClaimsPrincipal user)
    {
        Claim? identitfierClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (identitfierClaim == null)
        {
            throw new InvalidOperationException("Null identifier claim");
        }
        return new List<Claim>()
        {
            new Claim("id", identitfierClaim.Value),
        };
    }
}
