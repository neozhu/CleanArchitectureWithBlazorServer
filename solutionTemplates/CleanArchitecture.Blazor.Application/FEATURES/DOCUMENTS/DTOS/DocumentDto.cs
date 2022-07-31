// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.$safeprojectname$.Features.Documents.DTOs;

public partial class DocumentDto : IMapFrom<Document>
{
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Document, DocumentDto>()
           .ForMember(x => x.TenantName, s => s.MapFrom(y => y.Tenant.Name));
        profile.CreateMap<DocumentDto, Document>()
           .ForMember(x => x.Tenant, s => s.Ignore());
    }
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public string? URL { get; set; }
    public DocumentType DocumentType { get; set; } = DocumentType.Document;
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }
}
