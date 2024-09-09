using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public class UserProfileStateService
{
    private UserProfile _userProfile = new UserProfile() { Email="", UserId="", UserName="" };

    public UserProfile UserProfile
    {
        get => _userProfile;
        set
        {
            _userProfile = value;
            NotifyStateChanged();
        }
    }

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();

    public void UpdateUserProfile(string? profilePictureDataUrl, string? fullName,string? phoneNumber)
    {
        _userProfile.ProfilePictureDataUrl = profilePictureDataUrl;
        _userProfile.DisplayName = fullName;
        _userProfile.PhoneNumber = phoneNumber;
        NotifyStateChanged();
    }
}

