using System.ComponentModel.DataAnnotations;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

[Description("Users")]
public class ApplicationUserDto
{
    [Display(Name = "User Id")] 
    public string Id { get; set; } = string.Empty;

    [Display(Name = "User Name")]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "Display Name")]
    public string? DisplayName { get; set; }

    [Display(Name = "Provider")]
    public string? Provider { get; set; } = "Local";

    [Display(Name = "Tenant Id")]
    public string? TenantId { get; set; }

    [Display(Name = "Tenant")] 
    public TenantDto? Tenant { get; set; }

    [Display(Name = "Profile Photo")] 
    public string? ProfilePictureDataUrl { get; set; }

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Superior Id")] 
    public string? SuperiorId { get; set; }

    [Display(Name = "Superior")] 
    public ApplicationUserDto? Superior { get; set; }

    [Display(Name = "Assigned Roles")] 
    public string[]? AssignedRoles { get; set; }

    [Display(Name = "Default Role")] 
    public string? DefaultRole => AssignedRoles?.FirstOrDefault();

    [Display(Name = "Active")] public bool IsActive { get; set; }

    [Display(Name = "Is Live")] public bool IsLive { get; set; }

    [Display(Name = "Password")] public string? Password { get; set; }

    [Display(Name = "Confirm Password")] 
    public string? ConfirmPassword { get; set; }
    [Display(Name = "Email Confirmed")] 
    public bool EmailConfirmed { get; set; }

    [Display(Name = "Status")] 
    public DateTimeOffset? LockoutEnd { get; set; }
    [Display(Name = "Time Zone")]
    public string? TimeZoneId { get; set; }
    [Display(Name = "Local Time Offset")]
    public TimeSpan LocalTimeOffset => string.IsNullOrEmpty(TimeZoneId)
    ? TimeZoneInfo.Local.BaseUtcOffset
    : TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId).BaseUtcOffset;
    [Display(Name = "Language")]
    public string? LanguageCode { get; set; }
    [Display(Name = "Last Modified At")]
    public DateTime? LastModifiedAt { get; set; }

    [Display(Name = "Created At")]
    public DateTime? CreatedAt { get; set; }
    [Display(Name = "Tenants")]
    [CustomValidation(typeof(ApplicationUserDto), nameof(ValidateTenants))]
    public IReadOnlyCollection<TenantDto> Tenants { get; set; } = new List<TenantDto>();

    public static ValidationResult? ValidateTenants(IReadOnlyCollection<TenantDto> tenants, ValidationContext context)
        => tenants.Count > 0 ? ValidationResult.Success : new ValidationResult("Please select at least one tenant.");

    public UserProfile ToUserProfile()
    {
        return new UserProfile(
            UserId: Id,
            UserName: UserName,
            Email: Email,
            Provider: Provider,
            Superior: Superior,
            SuperiorId: SuperiorId,
            ProfilePictureDataUrl: ProfilePictureDataUrl,
            DisplayName: DisplayName,
            PhoneNumber: PhoneNumber,
            DefaultRole: DefaultRole,
            AssignedRoles: AssignedRoles,
            IsActive: IsActive,
            TenantId: TenantId,
            TimeZoneId: TimeZoneId,
            LanguageCode: LanguageCode,
            Tenant: Tenant,
            AvailableTenants: Tenants.Select(t => new TenantDto() { Id = t.Id, Name = t.Name, Description = t.Description }).ToList()
        );
    }

    public bool IsInRole(string role)
    {
        return AssignedRoles?.Contains(role) ?? false;
    }
}
