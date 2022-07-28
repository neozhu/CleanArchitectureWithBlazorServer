// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Customers.Caching;


namespace CleanArchitecture.Blazor.Application.Features.Customers.Commands.Delete;

    public class DeleteCustomerCommand: IRequest<Result>, ICacheInvalidator
    {
      public int[] Id {  get; }
      public string CacheKey => CustomerCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => CustomerCacheKey.SharedExpiryTokenSource();
      public DeleteCustomerCommand(int[] id)
      {
        Id = id;
      }
    }

    public class DeleteCustomerCommandHandler : 
                 IRequestHandler<DeleteCustomerCommand, Result>

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
        public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            //TODO:Implementing DeleteCheckedCustomersCommandHandler method 
            var items = await _context.Customers.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var item in items)
            {
			    // add delete domain events if this entity implement the IHasDomainEvent interface
				// item.AddDomainEvent(new DeletedEvent<Customer>(item));
                _context.Customers.Remove(item);
            }
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

    }

