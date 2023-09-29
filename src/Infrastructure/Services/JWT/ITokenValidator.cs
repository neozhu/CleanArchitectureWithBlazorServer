using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public interface ITokenValidator
{
    Task<TokenValidationResult> ValidateTokenAsync(string token);
}
