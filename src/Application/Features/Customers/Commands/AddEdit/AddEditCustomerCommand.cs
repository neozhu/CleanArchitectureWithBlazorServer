// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Features.Customers.Caching;
using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Customers.Commands.AddEdit;

    public class AddEditCustomerCommand: IMapFrom<CustomerDto>,ICacheInvalidatorRequest<Result<int>>
    {
          [Description("Id")]
    public int Id {get;set;} 
    [Description("Name")]
    public string Name {get;set;} = String.Empty; 
    [Description("Description")]
    public string? Description {get;set;} 


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
            // TODO: Implement AddEditCustomerCommandHandler method 
            var dto = _mapper.Map<CustomerDto>(request);
            if (request.Id > 0)
            {
                var item = await _context.Customers.FindAsync(new object[] { request.Id }, cancellationToken) ?? throw new NotFoundException($"Customer with id: [{request.Id}] not found.");
                item = _mapper.Map(dto, item);
				// raise a update domain event
				item.AddDomainEvent(new CustomerUpdatedEvent(item));
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
            else
            {
                var item = _mapper.Map<Customer>(dto);
                // raise a create domain event
				item.AddDomainEvent(new CustomerCreatedEvent(item));
                _context.Customers.Add(item);
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
           
        }
    }

