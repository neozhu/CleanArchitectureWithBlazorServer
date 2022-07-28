// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Customers.Caching;
namespace CleanArchitecture.Blazor.Application.Features.Customers.Commands.AddEdit;

    public class AddEditCustomerCommand: CustomerDto,IRequest<Result<int>>, ICacheInvalidator
    {
      public string CacheKey => CustomerCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => CustomerCacheKey.SharedExpiryTokenSource();
    }

    public class AddEditCustomerCommandHandler : IRequestHandler<AddEditCustomerCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditCustomerCommandHandler> _localizer;
        public AddEditCustomerCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<AddEditCustomerCommandHandler> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(AddEditCustomerCommand request, CancellationToken cancellationToken)
        {
            //TODO:Implementing AddEditCustomerCommandHandler method 
            if (request.Id > 0)
            {
                var item = await _context.Customers.FindAsync(new object[] { request.Id }, cancellationToken) ?? throw new NotFoundException("Customer {request.Id} Not Found.");
                item = _mapper.Map(request, item);
				// add update domain events if this entity implement the IHasDomainEvent interface
				// item.AddDomainEvent(new UpdatedEvent<Customer>(item));
                await _context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
            else
            {
                var item = _mapper.Map<Customer>(request);
                // add create domain events if this entity implement the IHasDomainEvent interface
				// item.AddDomainEvent(new CreatedEvent<Customer>(item));
                _context.Customers.Add(item);
                await _context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
           
        }
    }

