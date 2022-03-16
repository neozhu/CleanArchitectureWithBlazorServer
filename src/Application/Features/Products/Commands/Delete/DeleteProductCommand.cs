// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.



using CleanArchitecture.Blazor.Application.Features.Products.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Products.Commands.Delete;

public class DeleteProductCommand : IRequest<Result>, ICacheInvalidator

{
    public int[] Id { get; }
    public string CacheKey => ProductCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => ProductCacheKey.SharedExpiryTokenSource;
    public DeleteProductCommand(int[] id)
    {
        Id = id;
    }
}


public class DeleteProductCommandHandler :
             IRequestHandler<DeleteProductCommand, Result>
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
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {

        var items = await _context.Products.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            _context.Products.Remove(item);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }


}

