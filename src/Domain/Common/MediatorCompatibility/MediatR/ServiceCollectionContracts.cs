namespace MediatR
{
    public sealed class MediatRServiceConfiguration
    {
        public INotificationPublisher? NotificationPublisher { get; set; }

        public void RegisterServicesFromAssembly(System.Reflection.Assembly assembly)
        {
        }

        public void AddRequestPreProcessor(Type serviceType, Type implementationType)
        {
        }

        public void AddOpenBehavior(Type behaviorType)
        {
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatR(
            this IServiceCollection services,
            Action<MediatR.MediatRServiceConfiguration> configuration)
        {
            configuration(new MediatR.MediatRServiceConfiguration());
            return services;
        }
    }
}
