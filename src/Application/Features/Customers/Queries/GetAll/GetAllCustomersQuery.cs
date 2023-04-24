// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Customers.Caching;
using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Customers.Queries.GetAll;

    public class GetAllCustomersQuery : ICacheableRequest<IEnumerable<CustomerDto>>
    {
       [IgnoreFilter]
       public string CacheKey => CustomerCacheKey.GetAllCacheKey;
       [IgnoreFilter]
       public MemoryCacheEntryOptions? Options => CustomerCacheKey.MemoryCacheEntryOptions;
    }
    
    public class GetAllCustomersQueryHandler :
         IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetAllCustomersQueryHandler> _localizer;

        public GetAllCustomersQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetAllCustomersQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<IEnumerable<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement GetAllCustomersQueryHandler method 
            var data = await _context.Customers
                         .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                         .AsNoTracking()
                         .ToListAsync(cancellationToken);
            return data;
        }
    }


