using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class RoleService : IRoleService
{
    private const string CACHEKEY = "ALL-ApplicationRoleDto";
    private readonly IMapper _mapper;
    private readonly IFusionCache _fusionCache;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleService(
        IMapper mapper,
        IFusionCache fusionCache,
        IServiceScopeFactory scopeFactory)
    {
        _mapper = mapper;
        _fusionCache = fusionCache;
        var scope = scopeFactory.CreateScope();
        _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        DataSource = new List<ApplicationRoleDto>();
    }

    public List<ApplicationRoleDto> DataSource { get; private set; }

    public event Func<Task>? OnChange;

    public void Initialize()
    {
        DataSource = _fusionCache.GetOrSet(CACHEKEY,
                         _ => _roleManager.Roles
                             .ProjectTo<ApplicationRoleDto>(_mapper.ConfigurationProvider).OrderBy(x => x.TenantId).ThenBy(x => x.Name)
                             .ToList())
                     ?? new List<ApplicationRoleDto>();
        OnChange?.Invoke();
    }


    public void Refresh()
    {
        _fusionCache.Remove(CACHEKEY);
        DataSource = _fusionCache.GetOrSet(CACHEKEY,
                         _ => _roleManager.Roles
                             .ProjectTo<ApplicationRoleDto>(_mapper.ConfigurationProvider).OrderBy(x => x.TenantId).ThenBy(x => x.Name)
                             .ToList())
                     ?? new List<ApplicationRoleDto>();
        OnChange?.Invoke();
    }
}