using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public interface ILoginService
{
    Task<AuthenticatedUserResponse> LoginAsync(ApplicationUser user);
}
