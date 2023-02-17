// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Behaviours;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Application.Services.MultiTenant;
using CleanArchitecture.Blazor.Application.Services.Picklist;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CleanArchitecture.Blazor.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(config=> {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            config.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
            config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            config.AddOpenBehavior(typeof(MemoryCacheBehaviour<,>));
            config.AddOpenBehavior(typeof(CacheInvalidationBehaviour<,>));
            config.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
        });
        services.AddFluxor(options => {
            options.ScanAssemblies(Assembly.GetExecutingAssembly());
            options.UseReduxDevTools();
                });
        services.AddLazyCache();
        services.AddScoped<IPicklistService, PicklistService>();
        services.AddScoped<ITenantsService, TenantsService>();
        services.AddScoped<RegisterFormModelFluentValidator>();
        return services;
    }
   
}
