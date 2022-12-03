using BlazorState;

namespace CleanArchitecture.Blazor.Application.Features.ThemeProfiles;
public partial class ThemeProfilesState
{
    public class UpdateThemeProfilesAction : IAction
    {
        public UpdateThemeProfilesAction(UserPreferences userPreferences)
        {
            Preferences = userPreferences;
        }
        public UserPreferences Preferences { get; private set; }
    }
     
}
