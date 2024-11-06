using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.Tenants.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using Riok.Mapperly.Abstractions;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Mappers;
[Mapper]
public static partial class TenantMapper
{
    public static partial TenantDto ToDto(Tenant tenant);
    public static partial Tenant Map(TenantDto dto);
    public static partial Tenant Map(AddEditTenantCommand command);
    public static partial void MapTo(AddEditTenantCommand command, Tenant tenant);
    public static partial IQueryable<TenantDto> ProjectTo(this IQueryable<Tenant> q);
}
