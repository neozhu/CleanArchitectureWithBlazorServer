// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.$safeprojectname$.Common.Behaviours;
using CleanArchitecture.Blazor.$safeprojectname$.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.$safeprojectname$.Common.Security;
using CleanArchitecture.Blazor.$safeprojectname$.Services.MultiTenant;
using CleanArchitecture.Blazor.$safeprojectname$.Services.Picklist;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CleanArchitecture.Blazor.$safeprojectname$;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        services.AddLazyCache();
        services.AddScoped<IPicklistService, PicklistService>();
        services.AddScoped<ITenantsService, TenantsService>();
        services.AddScoped<RegisterFormModelFluentValidator>();
        return services;
    }
   
}
