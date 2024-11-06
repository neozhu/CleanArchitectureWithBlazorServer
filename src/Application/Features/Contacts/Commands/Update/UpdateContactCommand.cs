// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using CleanArchitecture.Blazor.Application.Features.Contacts.DTOs;
using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;
using CleanArchitecture.Blazor.Application.Features.Contacts.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.Contacts.Commands.Update;

public class UpdateContactCommand : ICacheInvalidatorRequest<Result<int>>
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

public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public UpdateContactCommandHandler(
        IApplicationDbContext context
        )
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {

        var item = await _context.Contacts.FindAsync(request.Id, cancellationToken);
        if (item == null)
        {
            return await Result<int>.FailureAsync($"Contact with id: [{request.Id}] not found.");
        }
        ContactMapper.MapToExisting(request, item);
        // raise a update domain event
        item.AddDomainEvent(new ContactUpdatedEvent(item));
        await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(item.Id);
    }
}

