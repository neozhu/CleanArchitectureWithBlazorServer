using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class UserDataSourceService : DataSourceServiceBase<ApplicationUserDto>, IDisposable
{
    private const string CACHEKEY = "ALL-ApplicationUserDto";
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _scopeFactory;

    public UserDataSourceService(
        IMapper mapper,
        IFusionCache fusionCache,
        IServiceScopeFactory scopeFactory)
        : base(fusionCache, CACHEKEY)
    {
        _mapper = mapper;
        _scopeFactory = scopeFactory;
    }

    protected override async Task<List<ApplicationUserDto>?> LoadAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        return await userManager.Users
            .Include(x => x.UserRoles).ThenInclude(x => x.Role)
            .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
            .OrderBy(x => x.UserName)
            .ToListAsync(cancellationToken);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
