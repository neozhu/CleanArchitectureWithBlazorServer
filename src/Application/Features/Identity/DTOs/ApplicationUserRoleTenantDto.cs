using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
public class ApplicationUserRoleTenantDto
{
    public virtual ApplicationUserDto User { get; set; } = default!;
    public virtual ApplicationRoleDto Role { get; set; } = default!;
    public virtual TenantDto Tenant { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string TenantId { get; set; } = default!;
    public string RoleId { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    [NotMapped] public string? UserName { get; set; }
    [NotMapped] public string? RoleName { get; set; }
    [NotMapped] public string? TenantName { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ApplicationUserRoleTenant, ApplicationUserRoleTenantDto>(MemberList.None)
                .ForMember(x => x.TenantName, s => s.MapFrom(y => (y.Tenant != null) ? y.Tenant.Name : y.TenantName))
                .ForMember(x => x.RoleName, s => s.MapFrom(y => (y.Role != null) ? y.Role.Name : y.RoleName))
                .ForMember(x => x.UserName, s => s.MapFrom(y => (y.User != null) ? y.User.UserName : y.UserName))
                .ForMember(x => x.UserId, s => s.MapFrom(y => (y.User != null) ? y.User.Id: y.UserId))
                .ForMember(x => x.Tenant, s => s.MapFrom(y => y.Tenant ))
                .ForMember(x => x.TenantId, s => s.MapFrom(y => (y.Tenant != null) ? y.Tenant.Id : y.TenantId))
                .ForMember(x => x.Role, s => s.MapFrom(y => y.Role))
                .ForMember(x => x.RoleId, s => s.MapFrom(y => (y.Role != null) ? y.Role.Id : y.RoleId))
                ;
        }
    }
}
