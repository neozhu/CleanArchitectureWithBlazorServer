// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Application.Features.Documents.DTOs;

public partial class DocumentDto : IMapFrom<Document>
{
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Document, DocumentDto>()
           .ForMember(x => x.DocumentTypeName, s => s.MapFrom(y => y.DocumentType.Name));
        profile.CreateMap<DocumentDto, Document>(MemberList.None);
    }
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsPublic { get; set; }
    public string URL { get; set; }
    public string CreatedBy { get; set; }
    public DateTime Created { get; set; }
    public int DocumentTypeId { get; set; }
    public string DocumentTypeName { get; set; }
}
