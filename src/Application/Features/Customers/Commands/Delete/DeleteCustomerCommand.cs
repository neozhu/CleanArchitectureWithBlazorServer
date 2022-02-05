// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.Customers.Caching;

namespace CleanArchitecture.Razor.Application.Features.Customers.Commands.Delete;

public class DeleteCustomerCommand : IRequest<Result>, ICacheInvalidator
{
    public int Id { get; set; }
    public string CacheKey => CustomerCacheKey.GetAllCacheKey;

    public CancellationTokenSource ResetCacheToken => CustomerCacheKey.ResetCacheToken;
}
public class DeleteCheckedCustomersCommand : IRequest<Result>, ICacheInvalidator
{
    public int[] Id { get; set; }
    public string CacheKey => CustomerCacheKey.GetAllCacheKey;

    public CancellationTokenSource ResetCacheToken => CustomerCacheKey.ResetCacheToken;
}

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result>,
    IRequestHandler<DeleteCheckedCustomersCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteCustomerCommandHandler(
        IApplicationDbContext context
        )
    {
        _context = context;
    }
    public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.Customers.FindAsync(new object[] { request.Id }, cancellationToken);
        _context.Customers.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> Handle(DeleteCheckedCustomersCommand request, CancellationToken cancellationToken)
    {
        var items = await _context.Customers.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            _context.Customers.Remove(item);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
