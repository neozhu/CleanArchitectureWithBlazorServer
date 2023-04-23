using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Features.Fluxor;
public static class Reducers
{
    [ReducerMethod]
    public static UserProfileState ReduceUserProfileUpdateAction(UserProfileState state, UserProfileUpdateAction action) => new(action.UserProfile);
}
