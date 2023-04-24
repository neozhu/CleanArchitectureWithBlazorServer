// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Products.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Products.Commands.Delete;

public class DeleteProductCommand : FilterBase, ICacheInvalidatorRequest<Result<int>>
{
    [ArraySearchFilter()]
    public int[] Id { get; }
    public string CacheKey => ProductCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => ProductCacheKey.SharedExpiryTokenSource();
    public DeleteProductCommand(int[] id)
    {
        Id = id;
    }
}


public class DeleteProductCommandHandler :
             IRequestHandler<DeleteProductCommand, Result<int>>
    {
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<DeleteProductCommandHandler> _localizer;
    public DeleteProductCommandHandler(
        IApplicationDbContext context,
        IStringLocalizer<DeleteProductCommandHandler> localizer,
         IMapper mapper
        )
    {
        _context = context;
        _localizer = localizer;
        _mapper = mapper;
    }
    public async Task<Result<int>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {

        var items = await _context.Products.ApplyFilter(request).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            item.AddDomainEvent(new DeletedEvent<Product>(item));
            _context.Products.Remove(item);
        }
        var result= await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(result);
    }


}

