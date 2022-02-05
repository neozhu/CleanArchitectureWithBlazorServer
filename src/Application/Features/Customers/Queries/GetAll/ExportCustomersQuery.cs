// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.Customers.DTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using CleanArchitecture.Razor.Application.Features.Customers.Caching;

namespace CleanArchitecture.Razor.Application.Features.Customers.Queries.GetAll;

public class GetAllCustomersQuery : IRequest<IEnumerable<CustomerDto>>, ICacheable
{
    public string CacheKey => CustomerCacheKey.GetAllCacheKey;

    public MemoryCacheEntryOptions Options => CustomerCacheKey.MemoryCacheEntryOptions;
}

public class GetAllCustomersQueryQueryHandler :
     IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetAllCustomersQueryQueryHandler> _localizer;

    public GetAllCustomersQueryQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetAllCustomersQueryQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public Task<IEnumerable<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
