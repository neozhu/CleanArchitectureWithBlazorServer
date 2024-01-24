// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

public class TenantPendingDto : TenantDto
{ }

[Description("Tenants")]
public class TenantDto
{
    [Description("Tenant Id")] public string Id { get; set; } = Guid.NewGuid().ToString();

    [Description("Tenant Name")] public string? Name { get; set; }

    [Description("Description")] public string? Description { get; set; }
    [Description("Tenant Type")] public byte Type { get; set; } = (byte)TenantTypeEnum.HospitalAndStaff;

    public DateTime CreatedDate { get; set; } = DateTime.Today;
    public DateTime? ModifiedLastDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? CreatedByUser { get; set; }//TODO make it non nullable,as always someone exists
    public string? ModifiedLastByUser { get; set; }
    public string? ApprovedByUser { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Tenant, TenantDto>().ReverseMap();
        }
    }
}