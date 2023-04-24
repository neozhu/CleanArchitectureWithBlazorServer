using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Features.Identity.Dto;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;
public class UserDataProvider : IUserDataProvider
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    public List<ApplicationUserDto> DataSource { get; private set; }

    public event Action? OnChange;

    public UserDataProvider(
        IMapper mapper,
        IServiceScopeFactory scopeFactory)
    {
        _mapper = mapper;
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        DataSource = new List<ApplicationUserDto>();
    }
    public void Initialize()
    {
        DataSource = _userManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).OrderBy(x=>x.UserName).ToList();
        OnChange?.Invoke();
    }

    public async Task InitializeAsync()
    {
        DataSource =await _userManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).OrderBy(x => x.UserName).ToListAsync();
        OnChange?.Invoke();
    }

    public Task Refresh()
    {
        OnChange?.Invoke();
        return Task.CompletedTask;
    }
}
