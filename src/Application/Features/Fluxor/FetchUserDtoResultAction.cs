using CleanArchitecture.Blazor.Application.Features.Identity.Dto;

namespace CleanArchitecture.Blazor.Application.Features.Fluxor;
public class FetchUserDtoResultAction
{
    public UserProfile UserProfile { get; }
    public FetchUserDtoResultAction(ApplicationUserDto dto)
    {
        UserProfile = new UserProfile()
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
            DefaultRole = dto.DefaultRole
        };
    }
}
