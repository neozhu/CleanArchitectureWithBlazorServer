using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

[Description("Users")]
public class ApplicationUserDto
{
    [Description("User Id")] public string Id { get; set; } = string.Empty;

    [Description("User Name")] public string UserName { get; set; } = string.Empty;

    [Description("Full Name")] public string? DisplayName { get; set; }

    [Description("Provider")] public string? Provider { get; set; } = "Local";

    [Description("Tenant Id")] public string? TenantId { get; set; }

    [Description("Tenant")] public TenantDto? Tenant { get; set; }

    [Description("Profile Photo")] public string? ProfilePictureDataUrl { get; set; }

    [Description("Email")] public string Email { get; set; } = string.Empty;

    [Description("Phone Number")] public string? PhoneNumber { get; set; }

    [Description("Superior Id")] public string? SuperiorId { get; set; }

    [Description("Superior")] public ApplicationUserDto? Superior { get; set; }

    [Description("Assigned Roles")] public string[]? AssignedRoles { get; set; }

    [Description("Default Role")] public string? DefaultRole => AssignedRoles?.FirstOrDefault();

    [Description("Active")] public bool IsActive { get; set; }

    [Description("Is Live")] public bool IsLive { get; set; }

    [Description("Password")] public string? Password { get; set; }

    [Description("Confirm Password")] public string? ConfirmPassword { get; set; }
    [Description("Email Confirmed")] public bool EmailConfirmed { get; set; }

    [Description("Status")] public DateTimeOffset? LockoutEnd { get; set; }
    [Description("Time Zone")]
    public string? TimeZoneId { get; set; }
    [Description("Local Time Offset")]
    public TimeSpan LocalTimeOffset => string.IsNullOrEmpty(TimeZoneId)
    ? TimeZoneInfo.Local.BaseUtcOffset
    : TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId).BaseUtcOffset;
    [Description("Language")]
    public string? LanguageCode { get; set; }
    [Description("Last Modified At")]
    public DateTime? LastModifiedAt { get; set; }

    [Description("Created At")]
    public DateTime? CreatedAt { get; set; }
  
 
    public UserProfile ToUserProfile()
    {
        return new UserProfile(
            UserId: Id,
            UserName: UserName,
            Email: Email,
            Provider: Provider,
            SuperiorName: Superior?.UserName,
            SuperiorId: SuperiorId,
            ProfilePictureDataUrl: ProfilePictureDataUrl,
            DisplayName: DisplayName,
            PhoneNumber: PhoneNumber,
            DefaultRole: DefaultRole,
            AssignedRoles: AssignedRoles,
            IsActive: IsActive,
            TenantId: TenantId,
            TenantName: Tenant?.Name,
            TimeZoneId: TimeZoneId,
            LanguageCode: LanguageCode
        );
    }

    public bool IsInRole(string role)
    {
        return AssignedRoles?.Contains(role) ?? false;
    }

#pragma warning disable CS8619
#pragma warning disable CS8601
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>(MemberList.None)
                .ForMember(x => x.LocalTimeOffset, s => s.Ignore())
                .ForMember(x => x.EmailConfirmed, s => s.MapFrom(y => y.EmailConfirmed))
                .ForMember(x => x.AssignedRoles, s => s.MapFrom(y => y.UserRoles.Select(r => r.Role.Name)))
                .ForMember(x => x.Superior, s => s.MapFrom(y => y.Superior != null ? new ApplicationUserDto()
                {
                    Id = y.Superior.Id,
                    UserName = y.Superior.UserName,
                    DisplayName = y.Superior.DisplayName,
                    Email = y.Superior.Email,
                    PhoneNumber = y.Superior.PhoneNumber,
                    ProfilePictureDataUrl = y.Superior.ProfilePictureDataUrl,
                    IsActive = y.Superior.IsActive,
                    TenantId = y.Superior.TenantId,
                    AssignedRoles = y.Superior.UserRoles.Select(r => r.Role.Name).ToArray(),
                    TimeZoneId = y.Superior.TimeZoneId,
                    LanguageCode = y.Superior.LanguageCode
                } : null));
                 
                 

        }
    }
}

public class ApplicationUserDtoValidator : AbstractValidator<ApplicationUserDto>
{
    private readonly IStringLocalizer<ApplicationUserDtoValidator> _localizer;

    public ApplicationUserDtoValidator(IStringLocalizer<ApplicationUserDtoValidator> localizer)
    {
        _localizer = localizer;
        RuleFor(v => v.TenantId)
            .MaximumLength(128).WithMessage(_localizer["Tenant id must be less than 128 characters"])
            .NotEmpty().WithMessage(_localizer["Tenant name cannot be empty"]);
        RuleFor(v => v.Provider)
            .MaximumLength(128).WithMessage(_localizer["Provider must be less than 100 characters"])
            .NotEmpty().WithMessage(_localizer["Provider cannot be empty"]);
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage(_localizer["User name cannot be empty"])
            .Length(2, 100).WithMessage(_localizer["User name must be between 2 and 100 characters"]);
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(_localizer["E-mail cannot be empty"])
            .MaximumLength(100).WithMessage(_localizer["E-mail must be less than 100 characters"])
            .EmailAddress().WithMessage(_localizer["E-mail must be a valid email address"]);

        RuleFor(x => x.DisplayName)
            .MaximumLength(128).WithMessage(_localizer["Full name must be less than 128 characters"]);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage(_localizer["Phone number must be less than 20 digits"]);
   
    }
}
