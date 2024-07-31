// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Contacts.DTOs;
using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;
namespace CleanArchitecture.Blazor.Application.Features.Contacts.Commands.AddEdit;

public class AddEditContactCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("Name")]
    public string Name {get;set;} = String.Empty; 
    [Description("Description")]
    public string? Description {get;set;} 
    [Description("Email")]
    public string? Email {get;set;} 
    [Description("Phone Number")]
    public string? PhoneNumber {get;set;} 
    [Description("Country")]
    public string? Country {get;set;} 


      public string CacheKey => ContactCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => ContactCacheKey.GetOrCreateTokenSource();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ContactDto, AddEditContactCommand>(MemberList.None);
            CreateMap<AddEditContactCommand,Contact>(MemberList.None);
         
        }
    }
}

    public class AddEditContactCommandHandler : IRequestHandler<AddEditContactCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditContactCommandHandler> _localizer;
        public AddEditContactCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<AddEditContactCommandHandler> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(AddEditContactCommand request, CancellationToken cancellationToken)
        {
            if (request.Id > 0)
            {
                var item = await _context.Contacts.FindAsync(new object[] { request.Id }, cancellationToken) ?? throw new NotFoundException($"Contact with id: [{request.Id}] not found.");
                item = _mapper.Map(request, item);
				// raise a update domain event
				item.AddDomainEvent(new ContactUpdatedEvent(item));
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
            else
            {
                var item = _mapper.Map<Contact>(request);
                // raise a create domain event
				item.AddDomainEvent(new ContactCreatedEvent(item));
                _context.Contacts.Add(item);
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
           
        }
    }

