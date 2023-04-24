// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Features.Customers.Caching;
using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Customers.Commands.Update;

    public class UpdateCustomerCommand: IMapFrom<CustomerDto>,ICacheInvalidatorRequest<Result<int>>
    {
            [Description("Id")]
    public int Id {get;set;} 
    [Description("Name")]
    public string Name {get;set;} = String.Empty; 
    [Description("Description")]
    public string? Description {get;set;} 

        public string CacheKey => CustomerCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => CustomerCacheKey.SharedExpiryTokenSource();
    }

    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<int>>
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
        public async Task<Result<int>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
           // TODO: Implement UpdateCustomerCommandHandler method 
           var item =await _context.Customers.FindAsync( new object[] { request.Id }, cancellationToken)?? throw new NotFoundException($"Customer with id: [{request.Id}] not found.");;
           var dto = _mapper.Map<CustomerDto>(request);
           item = _mapper.Map(dto, item);
		    // raise a update domain event
		   item.AddDomainEvent(new CustomerUpdatedEvent(item));
           await _context.SaveChangesAsync(cancellationToken);
           return await Result<int>.SuccessAsync(item.Id);
        }
    }

