// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;
using CleanArchitecture.Blazor.Application.Features.Contacts.Mappers;
namespace CleanArchitecture.Blazor.Application.Features.Contacts.Commands.AddEdit;

public class AddEditContactCommand : ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")]
    public int Id { get; set; }
    [Description("Name")]
    public string? Name { get; set; }
    [Description("Description")]
    public string? Description { get; set; }
    [Description("Email")]
    public string? Email { get; set; }
    [Description("Phone number")]
    public string? PhoneNumber { get; set; }
    [Description("Country")]
    public string? Country { get; set; }
    public string CacheKey => ContactCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => ContactCacheKey.GetOrCreateTokenSource();
}

public class AddEditContactCommandHandler : IRequestHandler<AddEditContactCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    public AddEditContactCommandHandler(
        IApplicationDbContext context
        )
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(AddEditContactCommand request, CancellationToken cancellationToken)
    {
        if (request.Id > 0)
        {
            var item = await _context.Contacts.FindAsync(request.Id, cancellationToken);
            if (item == null)
            {
                return await Result<int>.FailureAsync($"Contact with id: [{request.Id}] not found.");
            }
            ContactMapper.MapTo(request,item);
            // raise a update domain event
            item.AddDomainEvent(new ContactUpdatedEvent(item));
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            var item = ContactMapper.Map(request);
            // raise a create domain event
            item.AddDomainEvent(new ContactCreatedEvent(item));
            _context.Contacts.Add(item);
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
    }
}

