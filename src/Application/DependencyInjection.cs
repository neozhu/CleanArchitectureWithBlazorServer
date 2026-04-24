// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Common.PublishStrategies;
using CleanArchitecture.Blazor.Application.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Mediator;

namespace CleanArchitecture.Blazor.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(_ => MapsterConfiguration.Create());
        services.AddScoped<IObjectMapper, MapsterObjectMapper>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediator(options =>
        {
            options.Assemblies = [typeof(CleanArchitecture.Blazor.Application.DependencyInjection), typeof(CleanArchitecture.Blazor.Domain.Common.DomainEvent)];
            options.NotificationPublisherType = typeof(ChannelBasedNoWaitPublisher);
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors = [
                typeof(ValidationBehavior<,>),
                typeof(ResultExceptionBehavior<,>),
                typeof(PerformanceBehaviour<,>),
                typeof(FusionCacheBehaviour<,>),
                typeof(CacheInvalidationBehaviour<,>)
                ];

        });
       

        return services;
    }
}
