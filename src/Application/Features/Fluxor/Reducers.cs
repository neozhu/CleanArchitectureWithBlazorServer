namespace CleanArchitecture.Blazor.Application.Features.Fluxor;
public static class Reducers
{
    [ReducerMethod]
    public static UserProfileState ReduceUserProfileUpdateAction(UserProfileState state, UserProfileUpdateAction action) => new(false,action.UserProfile);
    [ReducerMethod]
    public static UserProfileState ReduceFetchUserDtoAction(UserProfileState state, FetchUserDtoAction action) => new(true, null);
    [ReducerMethod]
    public static UserProfileState ReduceFetchUserDtoResultAction(UserProfileState state, FetchUserDtoResultAction action) => new(false,action.UserProfile);
}
