// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Documents.Caching;
using CleanArchitecture.Blazor.Application.Features.Documents.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Commands.AddEdit;

public class AddEditDocumentCommand : ICacheInvalidatorRequest<Result<int>>
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
    public UploadRequest? UploadRequest { get; set; }
    public IEnumerable<string>? Tags => DocumentCacheKey.Tags;
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<DocumentDto, AddEditDocumentCommand>(MemberList.None);
            CreateMap<AddEditDocumentCommand, Document>(MemberList.None);
        }
    }
}

public class AddEditDocumentCommandHandler : IRequestHandler<AddEditDocumentCommand, Result<int>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<AddEditDocumentCommandHandler> _localizer;

    public AddEditDocumentCommandHandler(
        IApplicationDbContextFactory dbContextFactory,
        IMapper mapper,
        IStringLocalizer<AddEditDocumentCommandHandler> localizer
    )
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<Result<int>> Handle(AddEditDocumentCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        Document document;
        if (request.Id > 0)
        {
            var existingDocument = await db.Documents.FindAsync(request.Id, cancellationToken);
            if (existingDocument == null) return await Result<int>.FailureAsync(_localizer["Document Not Found!"]);
            document = _mapper.Map(request, existingDocument);
        }
        else
        {
            document = _mapper.Map<Document>(request);
            db.Documents.Add(document);
        }
        await db.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(document.Id);
    }
}
