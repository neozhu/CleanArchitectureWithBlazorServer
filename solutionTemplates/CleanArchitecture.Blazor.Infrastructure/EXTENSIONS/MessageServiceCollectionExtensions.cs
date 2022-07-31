using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Blazor.$safeprojectname$.Extensions;
public static class MessageServiceCollectionExtensions
{
    public static IServiceCollection AddMessageServices(this IServiceCollection services, IConfiguration configuration)
    {
        var mailSettings = new MailSettings();
        configuration.GetSection(MailSettings.SectionName).Bind(mailSettings);
        services.Configure<MailSettings>(configuration.GetSection(MailSettings.SectionName));
        services.AddSingleton(mailSettings);
        services.AddScoped<IMailService, SMTPMailService>();
        // configure your sender and template choices with dependency injection.
        services.AddFluentEmail(mailSettings.From)
                .AddRazorRenderer()
                .AddSmtpSender(new System.Net.Mail.SmtpClient()
                {
                    Host = mailSettings.Host,
                    Port = mailSettings.Port,
                    EnableSsl = mailSettings.UseSsl,
                    Credentials = new System.Net.NetworkCredential(mailSettings.UserName, mailSettings.Password)
                });
        return services;
    }
}
