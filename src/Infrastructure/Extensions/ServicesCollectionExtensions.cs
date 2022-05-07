using CleanArchitecture.Blazor.Infrastructure.Services.Authentication;
using CleanArchitecture.Blazor.Infrastructure.Services.Picklist;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
        => services.AddScoped<ExceptionHandlingMiddleware>()
                   .AddSingleton<ProfileService>()
                   .AddScoped<IPicklistService, PicklistService>()
                   .AddScoped<ICurrentUserService, CurrentUserService>()
                   .AddScoped<IDateTime, DateTimeService>()
                   .AddScoped<IExcelService, ExcelService>()
                   .AddScoped<IUploadService, UploadService>();
}
