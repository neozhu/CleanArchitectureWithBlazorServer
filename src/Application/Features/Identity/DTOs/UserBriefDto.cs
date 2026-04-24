using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

[Description("User summary")]
public class UserBriefDto
{
    [Display(Name = "User Id")]
    public string Id { get; set; } = string.Empty;

    [Display(Name = "User Name")]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "Display Name")]
    public string? DisplayName { get; set; }

    [Display(Name = "Profile Photo")]
    public string? ProfilePictureDataUrl { get; set; }}
