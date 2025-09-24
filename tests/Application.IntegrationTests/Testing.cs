using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Respawn;
using Respawn.Graph;

namespace CleanArchitecture.Blazor.Application.IntegrationTests;

[SetUpFixture]
public class Testing
{
    private static IConfigurationRoot _configuration;
    private static IServiceScopeFactory _scopeFactory;
    private static Respawner _checkpoint;
    private static string _currentUserId;

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables();

        _configuration = builder.Build();

        //var startup = new Startup(_configuration);

        var services = new ServiceCollection();

        services.AddSingleton(Mock.Of<IWebHostEnvironment>(w =>
            w.EnvironmentName == "Development" &&
            w.ApplicationName == "Server.UI"));

        services.AddInfrastructure(_configuration)
            .AddApplication();

        //services.AddLogging();

        //startup.ConfigureServices(services);

        // 替换 IUserContextAccessor 的注册
        var userContextServiceDescriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IUserContextAccessor));
        if (userContextServiceDescriptor != null)
        {
            services.Remove(userContextServiceDescriptor);
        }

        // 使用 Moq 创建 Mock 对象并配置 Current 属性
        services.AddSingleton<IUserContextAccessor>(provider =>
        {
            var mockUserContextAccessor = new Mock<IUserContextAccessor>();
            if (!string.IsNullOrEmpty(_currentUserId))
            {
                var userContext = new UserContext(_currentUserId, "admin", null, "admin@example.com");
                mockUserContextAccessor.Setup(x => x.Current).Returns(userContext);
            }
            return mockUserContextAccessor.Object;
        });

        _scopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();
        EnsureDatabase();
        _checkpoint = await Respawner.CreateAsync(
            _configuration.GetValue<string>("DatabaseSettings:ConnectionString"),
            new RespawnerOptions
            {
                TablesToIgnore = new Table[] { "__EFMigrationsHistory" }
            });

        
    }

    private static void EnsureDatabase()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        context.Database.Migrate();
    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetService<IMediator>();
        return await mediator.Send(request);
    }

    public static async Task<string> RunAsDefaultUserAsync()
    {
        return await RunAsUserAsync("Demo", "Password123!", new string[] { });
    }

    public static async Task<string> RunAsAdministratorAsync()
    {
        return await RunAsUserAsync("administrator", "Password123!", new[] { "Admin" });
    }

    public static async Task<string> RunAsUserAsync(string userName, string password, string[] roles)
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
        var user = new ApplicationUser { UserName = userName, Email = userName };
        var result = await userManager.CreateAsync(user, password);

        if (roles.Any())
        {
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
            await userManager.AddToRolesAsync(user, roles);
        }

        if (result.Succeeded)
        {
            _currentUserId = user.Id;
            return _currentUserId;
        }

        var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);
        throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
    }

    public static async Task ResetState()
    {
        await _checkpoint.ResetAsync(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        _currentUserId = null;
    }

    public static async Task<TEntity> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        return await context.FindAsync<TEntity>(keyValues);
    }
    public static IApplicationDbContext CreateDbContext()
    {
        var scope = _scopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        context.Add(entity);
        await context.SaveChangesAsync();
    }

    public static async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        return await context.Set<TEntity>().CountAsync();
    }

    public static IPicklistService CreatePicklistService()
    {
        var scope = _scopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IPicklistService>();
    }

    public static ITenantService CreateTenantsService()
    {
        var scope = _scopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ITenantService>();
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
    }
}
