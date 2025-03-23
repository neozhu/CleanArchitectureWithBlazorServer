﻿//------------------------------------------------------------------------------
// <auto-generated>
// CleanArchitecture.Blazor - MIT Licensed.
// Author: neozhu
// Created/Modified: 2025-03-19
// Command and handler for deleting Contact entities.
// Implements cache invalidation and triggers domain events.
// Docs: https://docs.cleanarchitectureblazor.com/features/contact
// </auto-generated>
//------------------------------------------------------------------------------

// Usage:
// Delete multiple Contacts by specifying their IDs.
// Domain events are raised for each deletion to support cache invalidation.


using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;


namespace CleanArchitecture.Blazor.Application.Features.Contacts.Commands.Delete;

public class DeleteContactCommand:  ICacheInvalidatorRequest<Result<int>>
{
  public int[] Id {  get; }
  public string CacheKey => ContactCacheKey.GetAllCacheKey;
  public IEnumerable<string>? Tags => ContactCacheKey.Tags;
  public DeleteContactCommand(int[] id)
  {
    Id = id;
  }
}

public class DeleteContactCommandHandler : 
             IRequestHandler<DeleteContactCommand, Result<int>>

{
    private readonly IApplicationDbContext _context;
    public DeleteContactCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
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

