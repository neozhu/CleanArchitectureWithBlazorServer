// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Customers.Caching;
using CleanArchitecture.Blazor.Application.Features.Customers.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Customers.Queries.GetById;

public class GetCustomerByIdQuery : ICacheableRequest<CustomerDto>
{
   public required int Id { get; set; }
   public string CacheKey => CustomerCacheKey.GetByIdCacheKey($"{Id}");
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
        var data = await _context.Customers.ApplySpecification(new CustomerByIdSpecification(request.Id))
                     .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                     .FirstAsync(cancellationToken) ?? throw new NotFoundException($"Customer with id: [{request.Id}] not found.");
        return data;
    }
}
