using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorState;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Profile;
public partial class UserProfileState
{
    public class UpdateUserProfileAction : IAction
    {
        public UpdateUserProfileAction(UserDto userDto)
        {
            UserDto = userDto;
        }
        public UserDto UserDto { get; set; }
    }
}
