namespace CleanArchitecture.Blazor.Server.UI.Fluxor;

public static class Reducers
{
    [ReducerMethod]
    public static UserProfileState ReduceUserProfileUpdateAction(UserProfileState state, UserProfileUpdateAction action)
    {
        return new UserProfileState(false, action.UserProfile);
    }

    [ReducerMethod]
    public static UserProfileState ReduceFetchUserDtoAction(UserProfileState state, FetchUserDtoAction action)
    {
        return new UserProfileState(true, null);
    }

    [ReducerMethod]
    public static UserProfileState ReduceFetchUserDtoResultAction(UserProfileState state,
        FetchUserDtoResultAction action)
    {
        return new UserProfileState(false, action.UserProfile);
    }
}