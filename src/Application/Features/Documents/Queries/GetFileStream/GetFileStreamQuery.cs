// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Documents.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Queries.GetFileStream;

public class GetFileStreamQuery : ICacheableRequest<(string, byte[])>
{
    public GetFileStreamQuery(int id)
    {
        Id = id;
    }

    public int Id { get; set; }

    public string CacheKey => DocumentCacheKey.GetStreamByIdKey(Id);
    public MemoryCacheEntryOptions? Options => DocumentCacheKey.MemoryCacheEntryOptions;
}

public class GetFileStreamQueryHandler : IRequestHandler<GetFileStreamQuery, (string, byte[])>
{
    private readonly IApplicationDbContext _context;


    public GetFileStreamQueryHandler(
        IApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<(string, byte[])> Handle(GetFileStreamQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.Documents.FindAsync(new object?[] { request.Id }, cancellationToken);
        if (item is null) throw new Exception($"not found document entry by Id:{request.Id}.");
        if (string.IsNullOrEmpty(item.URL)) return (string.Empty, Array.Empty<byte>());

        var filepath = Path.Combine(Directory.GetCurrentDirectory(), item.URL);
        if (!File.Exists(filepath)) return (string.Empty, Array.Empty<byte>());

        var fileName = new FileInfo(filepath).Name;
        var buffer = await File.ReadAllBytesAsync(filepath, cancellationToken);
        return (fileName, buffer);
    }

    internal class DocumentsQuery : Specification<Document>
    {
        public DocumentsQuery(string userId, string tenantId, string keyword)
        {
            Query.Where(p => (p.CreatedBy == userId && p.IsPublic == false) || p.IsPublic == true)
                .Where(x => x.TenantId == tenantId, !string.IsNullOrEmpty(tenantId))
                .Where(x => x.Title!.Contains(keyword) || x.Description!.Contains(keyword),
                    !string.IsNullOrEmpty(keyword));
        }
    }
}