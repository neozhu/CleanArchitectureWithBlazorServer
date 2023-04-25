using CleanArchitecture.Blazor.Application.Features.Identity.Dto;

namespace CleanArchitecture.Blazor.Application.Features.Fluxor;
[FeatureState]
public class UserProfileState
{
    public UserProfile UserProfile { get; }
    public bool IsLoading { get; }
    public UserProfileState()
    {
        IsLoading = true;
        UserProfile = new() { Email= "", UserId="",UserName= "" };
    }
    public UserProfileState(bool loading,UserProfile? userProfile)
    {
        IsLoading = loading;
        UserProfile = userProfile?? new() { Email = "", UserId = "", UserName = "" }; ;
    }
    public UserProfileState(ApplicationUserDto dto)
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
            IsActive = dto.IsActive,
            TenantId = dto.TenantId,
            TenantName = dto.TenantName,
            SuperiorId = dto.SuperiorId,
            SuperiorName = dto.SuperiorName,
            AssignedRoles = dto.AssignedRoles,
            DefaultRole = dto.DefaultRole
        };
    }
}
