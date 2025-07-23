using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class UserService : IUserService
{
    private const string CACHEKEY = "ALL-ApplicationUserDto";
    private readonly IMapper _mapper;
    private readonly IFusionCache _fusionCache;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(
        IMapper mapper,
        IFusionCache fusionCache,
        IServiceScopeFactory scopeFactory)
    {
        _mapper = mapper;
        _fusionCache = fusionCache;
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        DataSource = new List<ApplicationUserDto>();
    }

    public List<ApplicationUserDto> DataSource { get; private set; }

    public event Func<Task>? OnChange;

    public async Task InitializeAsync()
    {
        DataSource =await _fusionCache.GetOrSetAsync(CACHEKEY,
                         _ => _userManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role)
                             .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).OrderBy(x => x.UserName)
                             .ToListAsync())
                     ?? new List<ApplicationUserDto>();
        OnChange?.Invoke();
    }


    public async Task RefreshAsync()
    {
        _fusionCache.Remove(CACHEKEY);
        DataSource =await _fusionCache.GetOrSetAsync(CACHEKEY,
                         _ => _userManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role)
                             .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).OrderBy(x => x.UserName)
                             .ToListAsync())
                     ?? new List<ApplicationUserDto>();
        OnChange?.Invoke();
    }
}