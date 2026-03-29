// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.PublishStrategies;
using CleanArchitecture.Blazor.Application.Pipeline;
using CleanArchitecture.Blazor.Application.Pipeline.PreProcessors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CleanArchitecture.Blazor.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DbExceptionHandler<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(GlobalExceptionHandler<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ServerExceptionHandler<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationExceptionHandler<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPreProcessor<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FusionCacheBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehaviour<,>));
        services.AddScoped<UserProfileStateService>();
        return services;
    }
    public static void InitializeCacheFactory(this IHost host)
    {
        FusionCacheFactory.Configure(host.Services);
    }
}
