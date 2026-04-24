// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using System.ComponentModel.DataAnnotations; // IDataSourceService


namespace CleanArchitecture.Blazor.Application.Features.Tenants.Commands.AddEdit;

public class AddEditTenantCommand : ICacheInvalidatorRequest<Result<string>>
{
 [Display(Name="Tenant Id")] public string Id { get; set; } = Guid.NewGuid().ToString();
 [Display(Name="Tenant Name")] public string? Name { get; set; }
 [Display(Name = "Description")] public string? Description { get; set; }
 public string CacheKey => TenantCacheKey.GetAllCacheKey;
 public IEnumerable<string>? Tags => TenantCacheKey.Tags;}

public class AddEditTenantCommandHandler : IRequestHandler<AddEditTenantCommand, Result<string>>
{
 private readonly IApplicationDbContextFactory _dbContextFactory;
 private readonly IObjectMapper _objectMapper;
 private readonly IDataSourceService<TenantDto> _tenantsService;

 public AddEditTenantCommandHandler(
 IApplicationDbContextFactory dbContextFactory,
 IObjectMapper objectMapper,
 IDataSourceService<TenantDto> tenantsService
 )
 {
 _dbContextFactory = dbContextFactory;
 _objectMapper = objectMapper;
 _tenantsService = tenantsService;
 }

 public async ValueTask<Result<string>> Handle(AddEditTenantCommand request, CancellationToken cancellationToken)
 {
 await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
 var item = await db.Tenants.FindAsync(new object[] { request.Id }, cancellationToken);
 if (item is null)
 {
 item = _objectMapper.Map<Tenant>(request);
 await db.Tenants.AddAsync(item, cancellationToken);
 }
 else
 {
 item = _objectMapper.Map(request, item);
 }
 await db.SaveChangesAsync(cancellationToken);
 await _tenantsService.RefreshAsync();
 return await Result<string>.SuccessAsync(item.Id);
 }
}
