using CleanArchitecture.Blazor.Application.Features.Tenants.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Mappers;
#pragma warning disable RMG020
#pragma warning disable RMG012
[Mapper]
public static partial class TenantMapper
{
    public static partial TenantDto ToDto(Tenant tenant);
    public static partial Tenant Map(TenantDto dto);
    public static partial Tenant Map(AddEditTenantCommand command);
    public static partial void MapTo(AddEditTenantCommand command, Tenant tenant);
    public static partial AddEditTenantCommand ToEditCommand(TenantDto dto);
    public static partial IQueryable<TenantDto> ProjectTo(this IQueryable<Tenant> q);
}
