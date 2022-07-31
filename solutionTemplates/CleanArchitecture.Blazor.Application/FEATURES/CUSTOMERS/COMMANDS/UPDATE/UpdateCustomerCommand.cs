// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.$safeprojectname$.Features.Customers.DTOs;
using CleanArchitecture.Blazor.$safeprojectname$.Features.Customers.Caching;

namespace CleanArchitecture.Blazor.$safeprojectname$.Features.Customers.Commands.Update;

    public class UpdateCustomerCommand: CustomerDto,IRequest<Result>, ICacheInvalidator
    {
        public string CacheKey => CustomerCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => CustomerCacheKey.SharedExpiryTokenSource();
    }

    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<UpdateCustomerCommandHandler> _localizer;
        public UpdateCustomerCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<UpdateCustomerCommandHandler> localizer,
             IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
           //TODO:Implementing UpdateCustomerCommandHandler method 
           var item =await _context.Customers.FindAsync( new object[] { request.Id }, cancellationToken);
           if (item != null)
           {
                item = _mapper.Map(request, item);
				// add update domain events if this entity implement the IHasDomainEvent interface
				// item.AddDomainEvent(new UpdatedEvent<Customer>(item));
                await _context.SaveChangesAsync(cancellationToken);
           }
           return Result.Success();
        }
    }

