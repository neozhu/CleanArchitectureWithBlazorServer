using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;

namespace CleanArchitecture.Blazor.Application.Features.Fluxor;
[FeatureState]
public class UserProfileState
{
    public UserProfile UserProfile { get; }
    public UserProfileState()
    {
        UserProfile = new() { Email= "", UserId=Guid.NewGuid().ToString(),UserName= "" };
    }
    public UserProfileState(UserProfile userProfile)
    {
        UserProfile = userProfile;
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
