using CleanArchitecture.Blazor.Application.Features.Documents.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.Documents.DTOs;
using CleanArchitecture.Blazor.Application.Features.Identity.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Mappers;
#pragma warning disable RMG020
#pragma warning disable RMG012
[Mapper]
[UseStaticMapper(typeof(ApplicationUserMapper))]
public static partial class DocumentMapper
{
    [MapProperty("Tenant.Name", "TenantName")]
    public static partial DocumentDto ToDto(Document document);

    [MapperIgnoreSource(nameof(DocumentDto.CreatedByUser))]
    public static partial Document FromDto(DocumentDto dto);
    [MapperIgnoreSource(nameof(DocumentDto.CreatedByUser))]
    public static partial AddEditDocumentCommand ToEditCommand(DocumentDto dto);
    public static partial Document FromEditCommand(AddEditDocumentCommand command);
    public static partial void ApplyChangesFrom(AddEditDocumentCommand command, Document document);

    public static partial IQueryable<DocumentDto> ProjectTo(this IQueryable<Document> q);

     
}
