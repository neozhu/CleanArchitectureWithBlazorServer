using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class RoleService : IRoleService, IDisposable
{
    private const string CACHEKEY = "ALL-ApplicationRoleDto";
    private readonly IMapper _mapper;
    private readonly IFusionCache _fusionCache;
    private readonly IServiceScopeFactory _scopeFactory;

    public RoleService(
        IMapper mapper,
        IFusionCache fusionCache,
        IServiceScopeFactory scopeFactory)
    {
        _mapper = mapper;
        _fusionCache = fusionCache;
        _scopeFactory = scopeFactory;
        DataSource = new List<ApplicationRoleDto>();
    }

    public List<ApplicationRoleDto> DataSource { get; private set; }

    public event Func<Task>? OnChange;

    public async Task InitializeAsync()
    {
        DataSource = await _fusionCache.GetOrSetAsync(CACHEKEY,
                         async _ =>
                         {
                             using var scope = _scopeFactory.CreateScope();
                             var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                             return await roleManager.Roles
                                 .ProjectTo<ApplicationRoleDto>(_mapper.ConfigurationProvider)
                                 .OrderBy(x => x.TenantId)
                                 .ThenBy(x => x.Name)
                                 .ToListAsync();
                         })
                     ?? new List<ApplicationRoleDto>();
        OnChange?.Invoke();
    }


    public async Task RefreshAsync()
    {
        _fusionCache.Remove(CACHEKEY);
        DataSource = await _fusionCache.GetOrSetAsync(CACHEKEY,
                         async _ =>
                         {
                             using var scope = _scopeFactory.CreateScope();
                             var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                             return await roleManager.Roles
                                 .ProjectTo<ApplicationRoleDto>(_mapper.ConfigurationProvider)
                                 .OrderBy(x => x.TenantId)
                                 .ThenBy(x => x.Name)
                                 .ToListAsync();
                         })
                     ?? new List<ApplicationRoleDto>();
        OnChange?.Invoke();
    }

    public void Dispose()
    {
        // No long-lived resources to dispose
        GC.SuppressFinalize(this);
    }
}