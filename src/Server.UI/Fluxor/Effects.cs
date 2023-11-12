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
        var result = await _identityService.GetApplicationUserDto(action.UserId);
        if (result is not null)
            dispatcher.Dispatch(new FetchUserDtoResultAction(result));
    }
}