// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.DocumentTypes.Caching;

namespace CleanArchitecture.Razor.Application.Features.DocumentTypes.Commands.Delete;

public class DeleteDocumentTypeCommand : IRequest<Result>, ICacheInvalidator
{
    public int Id { get; set; }

    public string CacheKey => DocumentTypeCacheKey.GetAllCacheKey;

    public CancellationTokenSource ResetCacheToken => DocumentTypeCacheKey.ResetCacheToken;
}
public class DeleteCheckedDocumentTypesCommand : IRequest<Result>, ICacheInvalidator
{
    public int[] Id { get; set; }

    public string CacheKey => DocumentTypeCacheKey.GetAllCacheKey;

    public CancellationTokenSource ResetCacheToken => DocumentTypeCacheKey.ResetCacheToken;
}

public class DeleteDocumentTypeCommandHandler : IRequestHandler<DeleteDocumentTypeCommand, Result>,
    IRequestHandler<DeleteCheckedDocumentTypesCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteDocumentTypeCommandHandler(
        IApplicationDbContext context
        )
    {
        _context = context;
    }
    public async Task<Result> Handle(DeleteDocumentTypeCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.DocumentTypes.FindAsync(new object[] { request.Id }, cancellationToken);
        _context.DocumentTypes.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> Handle(DeleteCheckedDocumentTypesCommand request, CancellationToken cancellationToken)
    {
        var items = await _context.DocumentTypes.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            _context.DocumentTypes.Remove(item);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
