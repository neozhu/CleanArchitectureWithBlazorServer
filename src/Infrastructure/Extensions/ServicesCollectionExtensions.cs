using CleanArchitecture.Blazor.Infrastructure.Services.Authentication;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
        => services.AddScoped<ExceptionHandlingMiddleware>()
                   .AddSingleton<ProfileService>()
                   .AddScoped<ICurrentUserService, CurrentUserService>()
                   .AddScoped<IDateTime, DateTimeService>()
                   .AddScoped<IExcelService, ExcelService>()
                   .AddScoped<IUploadService, UploadService>();
}
