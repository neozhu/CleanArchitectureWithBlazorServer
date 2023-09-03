// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Dto;

[Description("Roles")]
public class ApplicationRoleDto
{
    [Description("Id")] public string Id { get; set; } = Guid.NewGuid().ToString();

    [Description("Name")] public string Name { get; set; } = string.Empty;

    [Description("Normalized Name")] public string? NormalizedName { get; set; }

    [Description("Description")] public string? Description { get; set; }

    [Description("Tenant Type")] public byte TenantType { get; set; } = (byte)Domain.Enums.TenantTypeEnum.Patient;
    public byte Level { get; set; } = 1;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ApplicationRole, ApplicationRoleDto>().ReverseMap();
        }
    }
}