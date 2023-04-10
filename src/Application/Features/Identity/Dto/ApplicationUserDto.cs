using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Dto;
[Description("Users")]
public class ApplicationUserDto:IMapFrom<ApplicationUser>
{
    public void Mapping(Profile profile)
    {
        profile.CreateMap<ApplicationUser, ApplicationUserDto>(MemberList.None)
           .ForMember(x => x.SuperiorName, s => s.MapFrom(y => y.Superior.UserName))
           .ForMember(x=>x.AssignedRoles,s=>s.MapFrom(y=>y.UserRoles.Select(r=>r.Role.Name)));
         
    }
    [Description("User Id")]
    public string Id { get; set; } = string.Empty;
    [Description("User Name")]
    public string UserName { get; set; } = string.Empty;
    [Description("Display Name")]
    public string? DisplayName { get; set; }
    [Description("Provider")]
    public string? Provider { get; set; } = "Local";
    [Description("Tenant Id")]
    public string? TenantId { get; set; }
    [Description("Tenant Name")]
    public string? TenantName { get; set; }
    [Description("Profile Photo")]
    public string? ProfilePictureDataUrl { get; set; }
    [Description("Email")]
    public string Email { get; set; } = string.Empty;
    [Description("Phone Number")]
    public string? PhoneNumber { get; set; }
    [Description("Superior Id")]
    public string? SuperiorId { get; set; }
    [Description("Superior Name")]
    public string? SuperiorName { get; set; }
    [Description("Assigned Roles")]
    public string[]? AssignedRoles { get; set; }
    [Description("Default Role")]
    public string? DefaultRole  => AssignedRoles?.FirstOrDefault();
    [Description("Is Active")]
    public bool IsActive { get; set; }
    [Description("Is Live")]
    public bool IsLive { get; set; }
    [Description("Password")]
    public string? Password { get; set; }
    [Description("Confirm Password")]
    public string? ConfirmPassword { get; set; }
    [Description("Status")]
    public  DateTimeOffset? LockoutEnd { get; set; }
    public UserProfile ToUserProfile() => new()
    {

        UserId = Id,
        ProfilePictureDataUrl = ProfilePictureDataUrl,
        Email = Email,
        PhoneNumber = PhoneNumber,
        DisplayName = DisplayName,
        Provider = Provider,
        UserName = UserName,
        TenantId = TenantId,
        TenantName = TenantName,
        SuperiorId = SuperiorId,
        SuperiorName = SuperiorName,
        AssignedRoles = AssignedRoles,
        DefaultRole = DefaultRole

    };
    public bool IsInRole(string role)=> AssignedRoles?.Contains(role)??false;
}
