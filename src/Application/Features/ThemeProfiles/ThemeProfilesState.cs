
using BlazorState;
namespace CleanArchitecture.Blazor.Application.Features.ThemeProfiles;


public partial class ThemeProfilesState : State<ThemeProfilesState>
{
    public ThemeProfilesState()
    {
        Preferences = new();
    }
    public UserPreferences Preferences { get; private set; }
    public override void Initialize() { }

    public class UserPreferences
    {
        /// <summary>
        /// Set the direction layout of the docs to RTL or LTR. If true RTL is used
        /// </summary>
        public bool RightToLeft { get; set; }

        /// <summary>
        /// If true DarkTheme is used. LightTheme otherwise
        /// </summary>
        public bool IsDarkMode { get; set; }
        public string PrimaryColor { get; set; } = "#2d4275";
        public string SecondaryColor { get; set; } = "#ff4081ff";
        public double BorderRadius { get; set; } = 4;
    }
}

