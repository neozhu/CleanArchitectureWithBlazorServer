using FluentEmail.MailKitSmtp;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
public static class MessageServiceCollectionExtensions
{
    public static IServiceCollection AddMessageServices(this IServiceCollection services, IConfiguration configuration)
    {
        var smtpClientOptions = new SmtpClientOptions();
        configuration.GetSection(nameof(SmtpClientOptions)).Bind(smtpClientOptions);
        services.Configure<SmtpClientOptions>(configuration.GetSection(nameof(SmtpClientOptions)));
        services.AddSingleton(smtpClientOptions);
        services.AddScoped<IMailService, MailService>();
        // configure your sender and template choices with dependency injection.
        services.AddFluentEmail(smtpClientOptions.User)
                .AddRazorRenderer(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "EmailTemplates"))
                .AddMailKitSender(smtpClientOptions);
        return services;
    }
}
