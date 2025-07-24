using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class UserService : IUserService, IDisposable
{
    private const string CACHEKEY = "ALL-ApplicationUserDto";
    private readonly IMapper _mapper;
    private readonly IFusionCache _fusionCache;
    private readonly IServiceScopeFactory _scopeFactory;

    public UserService(
        IMapper mapper,
        IFusionCache fusionCache,
        IServiceScopeFactory scopeFactory)
    {
        _mapper = mapper;
        _fusionCache = fusionCache;
        _scopeFactory = scopeFactory;
        DataSource = new List<ApplicationUserDto>();
    }

    public List<ApplicationUserDto> DataSource { get; private set; }

    public event Func<Task>? OnChange;

    public async Task InitializeAsync()
    {
        DataSource = await _fusionCache.GetOrSetAsync(CACHEKEY,
                         async _ =>
                         {
                             using var scope = _scopeFactory.CreateScope();
                             var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                             return await userManager.Users
                                 .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                                 .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
                                 .OrderBy(x => x.UserName)
                                 .ToListAsync();
                         })
                     ?? new List<ApplicationUserDto>();
        OnChange?.Invoke();
    }


    public async Task RefreshAsync()
    {
        _fusionCache.Remove(CACHEKEY);
        DataSource = await _fusionCache.GetOrSetAsync(CACHEKEY,
                         async _ =>
                         {
                             using var scope = _scopeFactory.CreateScope();
                             var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                             return await userManager.Users
                                 .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                                 .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
                                 .OrderBy(x => x.UserName)
                                 .ToListAsync();
                         })
                     ?? new List<ApplicationUserDto>();
        OnChange?.Invoke();
    }

    public void Dispose()
    {
        // No long-lived resources to dispose
        GC.SuppressFinalize(this);
    }
}