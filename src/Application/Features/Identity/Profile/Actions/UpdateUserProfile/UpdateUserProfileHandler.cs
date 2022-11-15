using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorState;
using static CleanArchitecture.Blazor.Application.Constants.Permission.Permissions;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Profile;

public partial class UserProfileState
{
    public class UpdateUserProfileHandler : ActionHandler<UpdateUserProfileAction>
    {
        public UpdateUserProfileHandler(IStore aStore) : base(aStore) { }

        UserProfileState UserProfileState => Store.GetState<UserProfileState>();

        public override Task<Unit> Handle(UpdateUserProfileAction updateAction, CancellationToken aCancellationToken)
        {
            var dto = updateAction.UserDto;
            UserProfileState.UserProfile = new UserProfile()
            {
                UserId = dto.Id,
                ProfilePictureDataUrl = dto.ProfilePictureDataUrl,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                DisplayName = dto.DisplayName,
                Provider = dto.Provider,
                UserName = dto.UserName,
                IsActive = dto.IsActive,
                TenantId = dto.TenantId,
                TenantName = dto.TenantName,
                SuperiorId = dto.SuperiorId,
                SuperiorName = dto.SuperiorName,
                AssignRoles = dto.AssignRoles,
                Role= dto.Role
            };
            return Unit.Task;
        }
    }
}
