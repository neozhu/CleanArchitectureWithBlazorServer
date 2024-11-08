// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;
using CleanArchitecture.Blazor.Application.Features.Contacts.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.Contacts.Commands.Create;

public class CreateContactCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("Name")]
    public string Name {get;set;} 
    [Description("Description")]
    public string? Description {get;set;} 
    [Description("Email")]
    public string? Email {get;set;} 
    [Description("Phone number")]
    public string? PhoneNumber {get;set;} 
    [Description("Country")]
    public string? Country {get;set;} 

      public string CacheKey => ContactCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => ContactCacheKey.GetOrCreateTokenSource();
}
    
    public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        public CreateContactCommandHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<int>> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
           var item = ContactMapper.FromCreateCommand(request);
           // raise a create domain event
	       item.AddDomainEvent(new ContactCreatedEvent(item));
           _context.Contacts.Add(item);
           await _context.SaveChangesAsync(cancellationToken);
           return  await Result<int>.SuccessAsync(item.Id);
        }
    }

