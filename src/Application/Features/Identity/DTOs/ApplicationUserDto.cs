using System.ComponentModel.DataAnnotations.Schema;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Domain.Identity;
using DocumentFormat.OpenXml.Drawing;
using Common;

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

    [Description("Default Role")] public string? DefaultRole { get; set; }  //=> AssignedRoles?.FirstOrDefault();//todo take max permission role

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

                .ForMember(x => x.TenantName, s => s.MapFrom(y => y.TenantName))
                //s.MapFrom(y =>  y.UserRoleTenants.Any(g => g.TenantId == y.TenantId) ? y.TenantName : y.UserRoleTenants.FirstOrDefault().TenantName))

                .ForMember(x => x.TenantId, s => s.MapFrom(y => y.TenantId))
                //s.MapFrom(y => y.UserRoleTenants.Any(g => g.TenantId == y.TenantId) ? y.TenantId : y.UserRoleTenants.FirstOrDefault().TenantId))

                //.ForMember(x => x.AssignedRoles, s =>
                //s.MapFrom(y => y.UserRoleTenants.Any(g => g.TenantId == y.TenantId) ?
                //y.UserRoleTenants.Where(g => g.TenantId == y.TenantId).Select(r => r.Role.Name) : y.UserRoleTenants.Select(r => r.Role.Name)))
                //above selects roles of default tenant only
                .ForMember(x => x.AssignedRoles, s =>
                s.MapFrom(y => y.UserRoleTenants.Count > 0 ?
                EnumExtensions.SortByEnum<string, RoleNamesEnum>(
                y.UserRoleTenants.Select(r => string.IsNullOrEmpty(r.RoleName) ? (r.Role != null ? r.Role.Name : "") : r.RoleName),true).ToArray() : null))

                 //.ForMember(x => x.DefaultRole, s =>
                 //s.MapFrom(y => y.UserRoleTenants.Any(g => g.TenantId == y.TenantId) ?
                 //y.UserRoleTenants.Where(g => g.TenantId == y.TenantId).Select(r => string.IsNullOrEmpty(r.RoleName) ? (r.Role != null ? r.Role.Name : null) : r.RoleName).First() : y.UserRoleTenants.Select(r => string.IsNullOrEmpty(r.RoleName) ? (r.Role != null ? r.Role.Name : null) : r.RoleName).Distinct().MaxEnumString<RoleNamesEnum>()))

                 .ForMember(x => x.DefaultRole, s =>
                s.MapFrom(y => y.UserRoleTenants.Any(g => g.TenantId == y.TenantId) ?
                y.UserRoleTenants.Where(g => g.TenantId == y.TenantId).Select(r => r.RoleName).First()
                : y.UserRoleTenants.Select(r => r.RoleName).MaxEnumString<RoleNamesEnum>()))

                .ForMember(x => x.IsUserTenantRolesActive, s => s.MapFrom(y => y.UserRoleTenants.Any(r => r.IsActive)))
                ;
        }
    }
}