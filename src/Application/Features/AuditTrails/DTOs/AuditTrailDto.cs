// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;

[Description("Audit Trails")]
public class AuditTrailDto
{
    [Description("Id")] public int Id { get; set; }

    [Description("User Id")] public string? UserId { get; set; }

    [Description("Audit Type")] public AuditType? AuditType { get; set; }

    [Description("Table Name")] public string? TableName { get; set; }

    [Description("Created DateTime")] public DateTime DateTime { get; set; }

    [Description("Old Values")] public Dictionary<string, object?>? OldValues { get; set; }

    [Description("New Values")] public Dictionary<string, object?>? NewValues { get; set; }

    [Description("Affected Columns")] public List<string>? AffectedColumns { get; set; }

    [Description("Primary Key")] public string PrimaryKey { get; set; } = default!;

    [Description("Show Details")] public bool ShowDetails { get; set; }

    [Description("Owner")] public ApplicationUserDto? Owner { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<AuditTrail, AuditTrailDto>(MemberList.None)
                .ForMember(x => x.PrimaryKey,
                    s => s.MapFrom(y => JsonSerializer.Serialize(y.PrimaryKey, DefaultJsonSerializerOptions.Options)));
        }
    }
}