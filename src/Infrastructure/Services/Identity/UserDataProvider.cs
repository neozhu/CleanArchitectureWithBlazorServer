using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using LazyCache;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class UserDataProvider : IUserDataProvider
{
    private const string CACHEKEY = "ALL-ApplicationUserDto";
    private readonly IAppCache _cache;
    private readonly IMapper _mapper;
    private readonly CustomUserManager _userManager;
    public List<ApplicationUserDto> DataSource { get; private set; }

    public UserDataProvider(
        IAppCache cache,
        IMapper mapper,
        IServiceScopeFactory scopeFactory)
    {
        _cache = cache;
        _mapper = mapper;
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<CustomUserManager>();
        DataSource = new List<ApplicationUserDto>();
    }

    public event Action? OnChange;

    public void Initialize()
    {
        DataSource = _cache.GetOrAdd(CACHEKEY,()=>_userManager.Users.Include(x => x.UserRoleTenants).ThenInclude(x => x.Role).ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).OrderBy(x=>x.UserName).ToList());
        OnChange?.Invoke();
    }

    public async Task InitializeAsync()
    {
        DataSource =await _cache.GetOrAddAsync(CACHEKEY,()=> _userManager.Users.Include(x => x.UserRoleTenants).ThenInclude(x => x.Role).ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).OrderBy(x => x.UserName).ToListAsync());
        OnChange?.Invoke();
    }

    public async Task Refresh()
    {
        _cache.Remove(CACHEKEY);
        DataSource = await _cache.GetOrAddAsync(CACHEKEY, () => _userManager.Users.Include(x => x.UserRoleTenants).ThenInclude(x => x.Role).ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).OrderBy(x => x.UserName).ToListAsync());
        OnChange?.Invoke();
    }
}