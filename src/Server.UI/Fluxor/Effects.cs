using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

namespace CleanArchitecture.Blazor.Server.UI.Fluxor;

public class Effects
{
    private readonly IIdentityService _identityService;

    public Effects(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [EffectMethod]
    public async Task HandleFetchDataAction(FetchUserDtoAction action, IDispatcher dispatcher)
    {
        var result = await _identityService.GetApplicationUserDto(action.UserName);
        dispatcher.Dispatch(new FetchUserDtoResultAction(result));
    }

    [EffectMethod]
    public Task HandleUserProfileChangedAction(UserProfileUpdateAction action, IDispatcher dispatcher)
    {
        _identityService.RemoveApplicationUserCache(action.UserProfile.UserName);
        return Task.CompletedTask;
    }
}