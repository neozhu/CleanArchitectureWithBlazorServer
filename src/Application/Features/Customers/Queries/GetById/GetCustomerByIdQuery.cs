// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Features.Customers.Caching;
using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Customers.Queries.GetById;

    public class GetCustomerByIdQuery :FilterBase, ICacheableRequest<CustomerDto>
    {
       [OperatorComparison(OperatorType.Equal)]
       public required int Id { get; set; }
       [IgnoreFilter]
       public string CacheKey => CustomerCacheKey.GetByIdCacheKey($"{Id}");
       [IgnoreFilter]
       public MemoryCacheEntryOptions? Options => CustomerCacheKey.MemoryCacheEntryOptions;
    }
    
    public class GetCustomerByIdQueryHandler :
         IRequestHandler<GetCustomerByIdQuery, CustomerDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetCustomerByIdQueryHandler> _localizer;

        public GetCustomerByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetCustomerByIdQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<CustomerDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement GetCustomerByIdQueryHandler method 
            var data = await _context.Customers.ApplyFilter(request)
                         .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                         .FirstAsync(cancellationToken) ?? throw new NotFoundException($"Customer with id: [{request.Id}] not found.");;
            return data;
        }
    }


