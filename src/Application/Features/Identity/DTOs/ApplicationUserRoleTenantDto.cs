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
    public string TenantId { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    [NotMapped] public string? UserName { get; set; }
    [NotMapped] public string? RoleName { get; set; }
    [NotMapped] public string? TenantName { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ApplicationUserRoleTenant, ApplicationUserRoleTenantDto>(MemberList.None)
                .ForMember(x => x.TenantName, s => s.MapFrom(y => (y.Tenant != null) ? y.Tenant.Name : null))
                .ForMember(x => x.RoleName, s => s.MapFrom(y => (y.Role != null) ? y.Role.Name : null))
                .ForMember(x => x.UserName, s => s.MapFrom(y => (y.User != null) ? y.User.UserName : null))
                ;
        }
    }
}
