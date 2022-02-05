// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.Customers.Caching;
using CleanArchitecture.Razor.Application.Features.Customers.DTOs;

namespace CleanArchitecture.Razor.Application.Features.Customers.Commands.AddEdit;

public class AddEditCustomerCommand : CustomerDto, IRequest<Result<int>>, IMapFrom<Customer>, ICacheInvalidator
{
    public string CacheKey => CustomerCacheKey.GetAllCacheKey;

    public CancellationTokenSource ResetCacheToken => CustomerCacheKey.ResetCacheToken;
}

public class AddEditCustomerCommandHandler : IRequestHandler<AddEditCustomerCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddEditCustomerCommandHandler(
         IApplicationDbContext context,
         IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<Result<int>> Handle(AddEditCustomerCommand request, CancellationToken cancellationToken)
    {

        if (request.Id > 0)
        {
            var customer = await _context.Customers.FindAsync(new object[] { request.Id }, cancellationToken);
            customer = _mapper.Map(request, customer);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(customer.Id);
        }
        else
        {
            var customer = _mapper.Map<Customer>(request);
            var createevent = new CustomerCreatedEvent(customer);
            customer.DomainEvents.Add(createevent);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(customer.Id);
        }


    }
}
