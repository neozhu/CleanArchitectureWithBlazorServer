using System.ComponentModel.DataAnnotations.Schema;
using CleanArchitecture.Blazor.Domain.Identity;
using DocumentFormat.OpenXml.Drawing;

namespace CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

[Description("Users")]
public class ApplicationUserDto
{
    [Description("User Id")] public string Id { get; set; } = string.Empty;

    [Description("User Name")] public string UserName { get; set; } = string.Empty;

    [Description("Display Name")] public string? DisplayName { get; set; }

    [Description("Provider")] public string? Provider { get; set; } = "Local";

    [Description("Tenant Id")] public string? TenantId { get; set; }

    [Description("Tenant Name")] public string? TenantName { get; set; }
    [Description("Is User-Tenant Roles Active")][NotMapped] public bool IsUserTenantRolesActive { get; set; } = true;

    [Description("Profile Photo")] public string? ProfilePictureDataUrl { get; set; }

    [Description("Email")] public string Email { get; set; } = string.Empty;

    [Description("Phone Number")] public string? PhoneNumber { get; set; }

    [Description("Superior Id")] public string? SuperiorId { get; set; }

    [Description("Superior Name")] public string? SuperiorName { get; set; }

    [Description("Assigned Roles")] public string[]? AssignedRoles { get; set; }

    [Description("User Roles and Tenants")]
    public ICollection<ApplicationUserRoleTenantDto> UserRoleTenants { get; set; }

    [Description("Default Role")] public string? DefaultRole => AssignedRoles?.FirstOrDefault();//todo take max permission role

    [Description("Is User Active")] public bool IsActive { get; set; }

    [Description("Is User Live")] public bool IsLive { get; set; }

    [Description("Password")] public string? Password { get; set; }

    [Description("Confirm Password")] public string? ConfirmPassword { get; set; }

    [Description("Status")] public DateTimeOffset? LockoutEnd { get; set; }

    public UserProfile ToUserProfile()
    {
        return new UserProfile
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
            DefaultRole = DefaultRole,
            UserRoleTenants = UserRoleTenants
        };
    }

    public bool IsInRole(string role)//todo had to add tenant id parameter to check
    {
        return AssignedRoles?.Contains(role) ?? false;
    }

    private class Mapping : Profile
    {
        public Mapping()
        {
            //todo this is not working need to check
            CreateMap<ApplicationUser, ApplicationUserDto>(MemberList.None)
                .ForMember(x => x.SuperiorName, s => s.MapFrom(y => y.Superior!.UserName))
                .ForMember(x => x.UserRoleTenants, s => s.MapFrom(c => c.UserRoleTenants))
                //todo need to make sure of this 

                .ForMember(x => x.TenantName, s =>
                s.MapFrom(y => y.UserRoleTenants.Any(g => g.TenantId == y.TenantId) ? y.TenantName : y.UserRoleTenants.FirstOrDefault().TenantName))
                .ForMember(x => x.TenantId, s =>
                s.MapFrom(y => y.UserRoleTenants.Any(g => g.TenantId == y.TenantId) ? y.TenantId : y.UserRoleTenants.FirstOrDefault().TenantId))

                .ForMember(x => x.AssignedRoles, s =>
                s.MapFrom(y => y.UserRoleTenants.Any(g => g.TenantId == y.TenantId) ?
                y.UserRoleTenants.Where(g => g.TenantId == y.TenantId).Select(r => r.Role.Name) : y.UserRoleTenants.Select(r => r.Role.Name)))


                .ForMember(x => x.IsUserTenantRolesActive, s => s.MapFrom(y => y.UserRoleTenants.Any(r => r.IsActive)))
                ;
        }
    }
}