namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
/// <summary>
/// Implements <see cref="IClaimsProvider"/> to define your claims provider
/// </summary>
/// <typeparam name="TUser"></typeparam>
public interface IClaimsProvider
{
    /// <summary>
    /// Use this method to get a list of claims for the given user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    IEnumerable<Claim> ProvideAccessClaims(ClaimsPrincipal user);

    /// <summary>
    /// Use this method to get a list of claims for the given user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    IEnumerable<Claim> ProvideRefreshClaims(ClaimsPrincipal user);
}
