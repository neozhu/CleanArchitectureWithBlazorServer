// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Documents.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Commands.Delete;

public class DeleteDocumentCommand : ICacheInvalidatorRequest<Result<int>>
{
    public DeleteDocumentCommand(int[] id)
    {
        Id = id;
    }

    public int[] Id { get; set; }
    public CancellationTokenSource? SharedExpiryTokenSource => DocumentCacheKey.GetOrCreateTokenSource();
}

public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Result<int>>

{
    private readonly IApplicationDbContext _context;

    public DeleteDocumentCommandHandler(
        IApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var items = await _context.Documents.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            item.AddDomainEvent(new DeletedEvent<Document>(item));
            _context.Documents.Remove(item);
        }

        var result = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(result);
    }
}