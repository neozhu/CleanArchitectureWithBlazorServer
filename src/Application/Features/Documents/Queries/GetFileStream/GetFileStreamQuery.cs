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
    public IEnumerable<string>? Tags => DocumentCacheKey.Tags;
}

public class GetFileStreamQueryHandler : IRequestHandler<GetFileStreamQuery, (string, byte[])>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;


    public GetFileStreamQueryHandler(
        IApplicationDbContextFactory dbContextFactory
    )
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<(string, byte[])> Handle(GetFileStreamQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var item = await db.Documents.FindAsync(new object?[] { request.Id }, cancellationToken);
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
            Query.Where(p => (p.CreatedById == userId && p.IsPublic == false) || p.IsPublic == true)
                .Where(x => x.TenantId == tenantId, !string.IsNullOrEmpty(tenantId))
                .Where(x => x.Title!.Contains(keyword) || x.Description!.Contains(keyword),
                    !string.IsNullOrEmpty(keyword));
        }
    }
}
