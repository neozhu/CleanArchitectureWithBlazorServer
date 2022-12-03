using BlazorState;

namespace CleanArchitecture.Blazor.Application.Features.ThemeProfiles;

public partial class ThemeProfilesState
{
    public class UpdateUserProfileHandler : ActionHandler<UpdateThemeProfilesAction>
    {
        public UpdateUserProfileHandler(IStore aStore) : base(aStore) { }

        ThemeProfilesState ThemeProfilesState => Store.GetState<ThemeProfilesState>();

        public override Task<Unit> Handle(UpdateThemeProfilesAction updateAction, CancellationToken aCancellationToken)
        {
            ThemeProfilesState.Preferences = updateAction.Preferences;
            return Unit.Task;
        }
        
    }

     
}
