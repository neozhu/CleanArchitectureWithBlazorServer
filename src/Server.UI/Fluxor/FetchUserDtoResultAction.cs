using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Enums;
using Common;

namespace CleanArchitecture.Blazor.Server.UI.Fluxor;

public class FetchUserDtoResultAction
{
    public FetchUserDtoResultAction(ApplicationUserDto dto)
    {
        UserProfile = new UserProfile
        {
            UserId = dto.Id,
            ProfilePictureDataUrl = dto.ProfilePictureDataUrl,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            DisplayName = dto.DisplayName,
            Provider = dto.Provider,
            UserName = dto.UserName,
            TenantId = dto.TenantId,
            TenantName = dto.TenantName,
            SuperiorId = dto.SuperiorId,
            SuperiorName = dto.SuperiorName,
            AssignedRoles = dto.AssignedRoles,
            DefaultRole = dto.DefaultRole//dto.AssignedRoles.MaxEnumString<RoleNamesEnum>() //
        };
        //if (dto.UserRoleTenants != null && dto.UserRoleTenants.Any())
        //{
        //    UserProfile.AssignedRoles = dto.UserRoleTenants.Where(x => x.TenantId == dto.TenantId).Select(x => x.RoleName).ToList().ToArray();
        //    UserProfile.DefaultRole = UserProfile.AssignedRoles.First();
        //}
    }

    public UserProfile UserProfile { get; }
}