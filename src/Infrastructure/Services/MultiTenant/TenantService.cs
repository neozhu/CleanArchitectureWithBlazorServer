using System.Linq.Dynamic.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Common.Extensions;
using LazyCache;

namespace CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;

public class TenantService : ITenantService
{
    private readonly IAppCache _cache;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TenantService(
        IAppCache cache,
        IServiceScopeFactory scopeFactory,
        IMapper mapper)
    {
        _cache = cache;
        var scope = scopeFactory.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        _mapper = mapper;
    }

    public event Action? OnChange;
    public List<TenantDto> DataSource { get; private set; } = new();

    //public List<TenantDto> GetAllowedTenants(string userTenantId)
    //{
    //    var userTenant = DataSource.Find(x => x.Id == userTenantId);
    //    if (userTenant != null)
    //    {
    //        var lessOrEquivalentTenants = DataSource.Where(x => x.Type <= userTenant.Type).ToList();
    //    }
    //    return null;
    //}
    public List<TenantDto> GetAllowedTenants(ApplicationUser user)
    {
        //return GetAllowedTenants(_mapper.Map<ApplicationUserDto>(user));//todo this is not working mapping
        var dto = new ApplicationUserDto()
        {
            TenantId = user.TenantId //,DefaultRole=user.
            , Id = user.Id
            , UserRoleTenants = _mapper.Map<List<ApplicationUserRoleTenantDto>>(user.UserRoleTenants)
        };
        return GetAllowedTenants(dto);
    }
    public List<TenantDto> GetAllowedTenants(ApplicationUserDto userDto)
    {
        //if no mapping exists
        if (userDto.UserRoleTenants == null || userDto.UserRoleTenants.Count == 0)
        {
            if (!userDto.TenantId.IsNullOrEmpty())
                return [DataSource.FirstOrDefault(x => x.Id == userDto.TenantId)];
            return null;
        }

        foreach (var t in userDto.UserRoleTenants)
        {
            t.Tenant = DataSource.Find(d => d.Id == t.TenantId);
        }

        //internal user,give all tenants
        if (userDto.DefaultRole == RoleNamesEnum.ROOTADMIN.ToString() || //ned to think more
            userDto.UserRoleTenants.Any(x => x.RoleName == RoleNamesEnum.ROOTADMIN.ToString()
            || userDto.UserRoleTenants.Any(x => x.Tenant != null && x.Tenant.Type == (byte)TenantTypeEnum.Internal)))
        {
            return DataSource;
        }

        //all other users their tenat + created/approved/modified
        var userTenantIds = userDto.UserRoleTenants.Select(t => t.TenantId);
        
        var myTenants = DataSource.Where(x => userTenantIds.Contains(x.Id))
            //.OrderByDescending(x => x.Type)//already datasource is in sorted order no need to sort again
            .ToList();
        var myApprovedTenants = DataSource.Where(x => x.CreatedByUser == userDto.Id || x.ApprovedByUser == userDto.Id || x.ModifiedLastByUser == userDto.Id)
            //.OrderByDescending(x => x.Type)//already datasource is in sorted order no need to sort again
            .ToList();
        var result = new List<TenantDto>();
        if (myTenants.Count != 0)
            result.AddRange(myTenants);

        if (myApprovedTenants.Count != 0)
            result.AddRange(myApprovedTenants);
        return result.DistinctBy(x => x.Id).ToList();
    }

    public async Task InitializeAsync()
    {
        DataSource = await _cache.GetOrAddAsync(TenantCacheKey.TenantsCacheKey,
            () => _context.Tenants.OrderByDescending(x => x.Type).ThenBy(x => x.Name)
                .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                .ToListAsync(),
            TenantCacheKey.MemoryCacheEntryOptions);
    }

    public void Initialize()
    {
        DataSource = _cache.GetOrAdd(TenantCacheKey.TenantsCacheKey,
            () => _context.Tenants.OrderBy(x => x.Name)
                .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                .ToList(),
            TenantCacheKey.MemoryCacheEntryOptions);
    }

    public async Task Refresh()
    {
        _cache.Remove(TenantCacheKey.TenantsCacheKey);
        DataSource = await _cache.GetOrAddAsync(TenantCacheKey.TenantsCacheKey,
            () => _context.Tenants.OrderBy(x => x.Name)
                .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                .ToListAsync(),
            TenantCacheKey.MemoryCacheEntryOptions);
        OnChange?.Invoke();
    }
}