// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Commands.Delete;

public class DeleteTenantCommand : ICacheInvalidatorRequest<Result<int>>
{
    public string[] Id { get; }
    public string CacheKey => TenantCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => TenantCacheKey.SharedExpiryTokenSource();
    public DeleteTenantCommand(string[] id)
    {
        Id = id;
    }
}

public class DeleteTenantCommandHandler :
             IRequestHandler<DeleteTenantCommand, Result<int>>

{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<DeleteTenantCommandHandler> _localizer;
    public DeleteTenantCommandHandler(
        IApplicationDbContext context,
        IStringLocalizer<DeleteTenantCommandHandler> localizer,
         IMapper mapper
        )
    {
        _context = context;
        _localizer = localizer;
        _mapper = mapper;
    }
    public async Task<Result<int>> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
    {
        var items = await _context.Tenants.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            item.AddDomainEvent(new UpdatedEvent<Tenant>(item));
            _context.Tenants.Remove(item);
        }
        var result=  await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(result);
    }

}

