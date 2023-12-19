// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Documents.DTOs;

[Description("Documents")]
public class DocumentDto
{
    [Description("Id")] public int Id { get; set; }

    [Description("Title")] public string? Title { get; set; }

    [Description("Description")] public string? Description { get; set; }

    [Description("Is Public")] public bool IsPublic { get; set; }

    [Description("URL")] public string? URL { get; set; }

    [Description("Document Type")] public DocumentType DocumentType { get; set; } = DocumentType.Document;

    [Description("Tenant Id")] public string? TenantId { get; set; }

    [Description("Tenant Name")] public string? TenantName { get; set; }

    [Description("Status")] public JobStatus Status { get; set; } = JobStatus.NotStart;

    [Description("Content")] public string? Content { get; set; }

    [Description("Owner")] public ApplicationUserDto? Owner { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Document, DocumentDto>(MemberList.None)
                .ForMember(x => x.TenantName, s => s.MapFrom(y => y.Tenant!.Name));
            CreateMap<DocumentDto, Document>(MemberList.None)
                .ForMember(x => x.Tenant, s => s.Ignore())
                .ForMember(x => x.Owner, s => s.Ignore());
        }
    }
}