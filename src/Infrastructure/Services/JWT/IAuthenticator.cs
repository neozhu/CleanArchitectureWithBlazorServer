using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;
using CleanArchitecture.Blazor.Domain.Features.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public interface IAuthenticator
{
    Task<ApplicationUser?> Authenticate(string username, string password);
    Task<string> Refresh(string refreshToken);
}
