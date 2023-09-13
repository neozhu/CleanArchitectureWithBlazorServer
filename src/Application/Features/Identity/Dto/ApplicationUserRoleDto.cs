using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Dto;
public class ApplicationUserRoleDto
{
    public virtual ApplicationUserDto User { get; set; } = default!;
    public virtual ApplicationRoleDto Role { get; set; } = default!;
    public virtual Tenant Tenant { get; set; } = default!;
    public string TenantId { get; set; } = default!;
    public bool IsActive { get; set; } = true;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ApplicationUserRole, ApplicationUserRoleDto>(MemberList.None);
        }
    }
}
