﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;


namespace CleanArchitecture.Blazor.Application.Features.Tenants.Commands.AddEdit;

public class AddEditTenantCommand : ICacheInvalidatorRequest<Result<string>>
{
    [Description("Tenant Id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Description("Tenant Name")] public string? Name { get; set; }
    [Description("Description")] public string? Description { get; set; }
    public string CacheKey => TenantCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => TenantCacheKey.Tags;
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TenantDto, AddEditTenantCommand>(MemberList.None);
            CreateMap<AddEditTenantCommand, Tenant>(MemberList.None);
        }
    }
}

public class AddEditTenantCommandHandler : IRequestHandler<AddEditTenantCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ITenantService _tenantsService;

    public AddEditTenantCommandHandler(
        IMapper mapper,
        ITenantService tenantsService,
        IApplicationDbContext context
    )
    {
        _mapper = mapper;
        _tenantsService = tenantsService;
        _context = context;
    }

    public async Task<Result<string>> Handle(AddEditTenantCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.Tenants.FindAsync(new object[] { request.Id }, cancellationToken);
        if (item is null)
        {
            item = _mapper.Map<Tenant>(request);
            await _context.Tenants.AddAsync(item, cancellationToken);
        }
        else
        {
            item = _mapper.Map(request, item);
        }
        await _context.SaveChangesAsync(cancellationToken);
        _tenantsService.Refresh();
        return await Result<string>.SuccessAsync(item.Id);
    }
}