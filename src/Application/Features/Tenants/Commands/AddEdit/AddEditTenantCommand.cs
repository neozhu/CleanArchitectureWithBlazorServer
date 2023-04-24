// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Commands.AddEdit;

public class AddEditTenantCommand : IMapFrom<TenantDto>, ICacheInvalidatorRequest<Result<string>>
{
    [Description("Tenant Id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Description("Tenant Name")]
    public string? Name { get; set; }
    [Description("Description")]
    public string? Description { get; set; }
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
        var dto= _mapper.Map<TenantDto>(request);
        var item = await _context.Tenants.FindAsync(new object[] { request.Id }, cancellationToken);
        if(item is null)
        {
            item = _mapper.Map<Tenant>(dto);
            await _context.Tenants.AddAsync(item);
        }
        else
        {
            item = _mapper.Map(dto, item);
        }
        item.AddDomainEvent(new UpdatedEvent<Tenant>(item));
        await _context.SaveChangesAsync(cancellationToken);
        return Result<string>.Success(item.Id);


    }
}

