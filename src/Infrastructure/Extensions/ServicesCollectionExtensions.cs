
namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
        => services.AddScoped<ExceptionHandlingMiddleware>()
                   .AddScoped<ICurrentUserService, CurrentUserService>()
                   .AddScoped<IDateTime, DateTimeService>()
                   .AddScoped<IExcelService, ExcelService>()
                   .AddScoped<IUploadService, UploadService>();
}
