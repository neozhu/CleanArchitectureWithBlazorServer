// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;


namespace CleanArchitecture.Blazor.Application.Features.Contacts.Commands.Delete;

    public class DeleteContactCommand:  ICacheInvalidatorRequest<Result<int>>
    {
      public int[] Id {  get; }
      public string CacheKey => ContactCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => ContactCacheKey.GetOrCreateTokenSource();
      public DeleteContactCommand(int[] id)
      {
        Id = id;
      }
    }

    public class DeleteContactCommandHandler : 
                 IRequestHandler<DeleteContactCommand, Result<int>>

    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<DeleteContactCommandHandler> _localizer;
        public DeleteContactCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteContactCommandHandler> localizer,
             IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            var items = await _context.Contacts.Where(x=>request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var item in items)
            {
			    // raise a delete domain event
				item.AddDomainEvent(new ContactDeletedEvent(item));
                _context.Contacts.Remove(item);
            }
            var result = await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(result);
        }

    }

