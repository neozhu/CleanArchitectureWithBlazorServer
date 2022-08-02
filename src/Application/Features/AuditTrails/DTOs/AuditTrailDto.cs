// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;

public class AuditTrailDto : IMapFrom<AuditTrail>
{
    public void Mapping(Profile profile)
    {
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs)
        };
        profile.CreateMap<AuditTrail, AuditTrailDto>()
           .ForMember(x => x.AuditType, s => s.MapFrom(y => y.AuditType.ToString()))
           .ForMember(x => x.OldValues, s => s.MapFrom(y => JsonSerializer.Serialize(y.OldValues, options)))
           .ForMember(x => x.NewValues, s => s.MapFrom(y => JsonSerializer.Serialize(y.NewValues, options)))
           .ForMember(x => x.PrimaryKey, s => s.MapFrom(y => JsonSerializer.Serialize(y.PrimaryKey, options)))
           .ForMember(x => x.AffectedColumns, s => s.MapFrom(y => JsonSerializer.Serialize(y.AffectedColumns, options)))
           ;

    }
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string? AuditType { get; set; }
    public string? TableName { get; set; }
    public DateTime DateTime { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public string PrimaryKey { get; set; } = default!;
    public bool ShowDetails { get; set; }
}
