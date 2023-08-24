using CleanArchitecture.Blazor.Application.Features.Identity.Dto;

namespace CleanArchitecture.Blazor.Infrastructure;
public class StaticData//only for first time
{
    public static List<ApplicationRoleDto>? Roles = null;
    readonly IIdentityService _identityService;
    public StaticData(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task<List<ApplicationRoleDto>> LoadUserBaseRoles(bool forceLoad = false)
    {
        if (forceLoad || Roles == null || !Roles.Any())
            Roles = await _identityService.GetAllRoles();
        return Roles;
    }
}
