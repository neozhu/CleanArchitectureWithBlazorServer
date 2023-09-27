using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
/// <summary>
/// the dto used to send an authenticated user response containing access Token and refresh Token
/// </summary>
public class AuthenticatedUserResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
