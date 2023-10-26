using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Features.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public interface ILoginService
{
    Task<AuthenticatedUserResponse> LoginAsync(ClaimsPrincipal user);
}
