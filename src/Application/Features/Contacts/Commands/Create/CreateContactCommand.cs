﻿//------------------------------------------------------------------------------
// <auto-generated>
// CleanArchitecture.Blazor - MIT Licensed.
// Author: neozhu
// Created/Modified: 2025-03-19
// Command and handler for creating a new Contact.
// Uses caching invalidation and domain events for data consistency.
// Docs: https://docs.cleanarchitectureblazor.com/features/contact
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
#nullable disable warnings


using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Contacts.Commands.Create;

public class CreateContactCommand : ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")]
    public int Id { get; set; }
    [Description("Name")]
    public string Name { get; set; }
    [Description("Description")]
    public string? Description { get; set; }
    [Description("Email")]
    public string? Email { get; set; }
    [Description("Phone number")]
    public string? PhoneNumber { get; set; }
    [Description("Country")]
    public string? Country { get; set; }

    public string CacheKey => ContactCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => ContactCacheKey.Tags;
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateContactCommand, Contact>(MemberList.None);
        }
    }
}

public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, Result<int>>
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;
    public CreateContactCommandHandler(
        IMapper mapper,
        IApplicationDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }
    public async Task<Result<int>> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        var item = _mapper.Map<Contact>(request);
        // raise a create domain event
        item.AddDomainEvent(new ContactCreatedEvent(item));
        _context.Contacts.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(item.Id);
    }
}

