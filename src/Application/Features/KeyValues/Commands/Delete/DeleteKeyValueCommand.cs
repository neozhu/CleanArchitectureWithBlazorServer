// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Application.Features.KeyValues.Commands.Delete;

public class DeleteKeyValueCommand : IRequest<Result>
{
    public int Id { get; set; }
}
public class DeleteCheckedKeyValuesCommand : IRequest<Result>
{
    public int[] Id { get; set; }
}

public class DeleteKeyValueCommandHandler : IRequestHandler<DeleteKeyValueCommand, Result>,
    IRequestHandler<DeleteCheckedKeyValuesCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteKeyValueCommandHandler(
        IApplicationDbContext context
        )
    {
        _context = context;
    }
    public async Task<Result> Handle(DeleteKeyValueCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.KeyValues.FindAsync(new object[] { request.Id }, cancellationToken);
        _context.KeyValues.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> Handle(DeleteCheckedKeyValuesCommand request, CancellationToken cancellationToken)
    {
        var items = await _context.KeyValues.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            _context.KeyValues.Remove(item);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
