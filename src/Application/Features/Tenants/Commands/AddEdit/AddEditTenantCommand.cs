// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
namespace CleanArchitecture.Blazor.Application.Features.Tenants.Commands.AddEdit;

public class AddEditTenantCommand : TenantDto, IRequest<Result<string>>, ICacheInvalidator
{
    public string CacheKey => TenantCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => TenantCacheKey.SharedExpiryTokenSource();
}

public class AddEditTenantCommandHandler : IRequestHandler<AddEditTenantCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<AddEditTenantCommandHandler> _localizer;
    public AddEditTenantCommandHandler(
        IApplicationDbContext context,
        IStringLocalizer<AddEditTenantCommandHandler> localizer,
        IMapper mapper
        )
    {
        _context = context;
        _localizer = localizer;
        _mapper = mapper;
    }
    public async Task<Result<string>> Handle(AddEditTenantCommand request, CancellationToken cancellationToken)
    {

        var item = await _context.Tenants.FindAsync(new object[] { request.Id }, cancellationToken);
        if(item is null)
        {
            item = new Tenant()
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
            };
            await _context.Tenants.AddAsync(item);
        }
        else
        {
            item = _mapper.Map(request, item);
        }
        item.DomainEvents.Add(new UpdatedEvent<Tenant>(item));
        await _context.SaveChangesAsync(cancellationToken);
        return Result<string>.Success(item.Id);


    }
}

