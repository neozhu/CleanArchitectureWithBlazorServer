// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Customers.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Customers.Commands.Delete;

    public class DeleteCustomerCommand: FilterBase, ICacheInvalidatorRequest<Result<int>>
    {
      [ArraySearchFilter()]
      public int[] Id {  get; }
      public string CacheKey => CustomerCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => CustomerCacheKey.SharedExpiryTokenSource();
      public DeleteCustomerCommand(int[] id)
      {
        Id = id;
      }
    }

    public class DeleteCustomerCommandHandler : 
                 IRequestHandler<DeleteCustomerCommand, Result<int>>

    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<DeleteCustomerCommandHandler> _localizer;
        public DeleteCustomerCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteCustomerCommandHandler> localizer,
             IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement DeleteCheckedCustomersCommandHandler method 
            var items = await _context.Customers.ApplyFilter(request).ToListAsync(cancellationToken);
            foreach (var item in items)
            {
			    // raise a delete domain event
				item.AddDomainEvent(new CustomerDeletedEvent(item));
                _context.Customers.Remove(item);
            }
            var result = await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(result);
        }

    }

