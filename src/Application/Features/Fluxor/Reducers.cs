namespace CleanArchitecture.Blazor.Application.Features.Fluxor;
public static class Reducers
{
    [ReducerMethod]
    public static UserProfileState ReduceUserProfileUpdateAction(UserProfileState state, UserProfileUpdateAction action) => new(action.UserProfile);
    [ReducerMethod]
    public static UserProfileState ReduceFetchUserDtoResultAction(UserProfileState state, FetchUserDtoResultAction action) => new(action.UserProfile);
}
