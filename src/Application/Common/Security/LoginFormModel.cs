using System.ComponentModel.DataAnnotations;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Application.Common.Security;

public class LoginFormModel
{
    [Required(ErrorMessage = "User name cannot be empty")]
    [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
    public string UserName { get; set; } = "";
    [Required(ErrorMessage = "Password cannot be empty")]
    [StringLength(30, ErrorMessage = "Password must be at least 6 characters long.", MinimumLength = 6)]
    public string Password { get; set; } = "";
    public bool RememberMe { get; set; } = false;
}

