// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;
[Description("Audit Trails")]
public class AuditTrailDto : IMapFrom<AuditTrail>
{
    public void Mapping(Profile profile)
    {
        profile.CreateMap<AuditTrail, AuditTrailDto>( MemberList.None)
           .ForMember(x => x.OldValues, s => s.MapFrom(y => JsonSerializer.Serialize(y.OldValues, DefaultJsonSerializerOptions.Options)))
           .ForMember(x => x.NewValues, s => s.MapFrom(y => JsonSerializer.Serialize(y.NewValues, DefaultJsonSerializerOptions.Options)))
           .ForMember(x => x.PrimaryKey, s => s.MapFrom(y => JsonSerializer.Serialize(y.PrimaryKey, DefaultJsonSerializerOptions.Options)))
           .ForMember(x => x.AffectedColumns, s => s.MapFrom(y => JsonSerializer.Serialize(y.AffectedColumns, DefaultJsonSerializerOptions.Options)));

    }
    [Description("Id")]
    public int Id { get; set; }
    [Description("User Id")]
    public string? UserId { get; set; }
    [Description("Audit Type")]
    public AuditType? AuditType { get; set; }
    [Description("Table Name")]
    public string? TableName { get; set; }
    [Description("Created DateTime")]
    public DateTime DateTime { get; set; }
    [Description("Old Values")]
    public string? OldValues { get; set; }
    [Description("New Values")]
    public string? NewValues { get; set; }
    [Description("Affected Columns")]
    public string? AffectedColumns { get; set; }
    [Description("Primary Key")]
    public string PrimaryKey { get; set; } = default!;
    [Description("Show Details")]
    public bool ShowDetails { get; set; }
    [Description("Owner")]
    public ApplicationUserDto? Owner { get; set; }
}
