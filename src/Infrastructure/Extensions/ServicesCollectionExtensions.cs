using CleanArchitecture.Blazor.Application.Services.PaddleOCR;
using CleanArchitecture.Blazor.Infrastructure.Services.Authentication;
using CleanArchitecture.Blazor.Infrastructure.Services.PaddleOCR;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
        => services.AddScoped<ExceptionHandlingMiddleware>()
                   .AddScoped<ICurrentUserService, CurrentUserService>()
                   .AddScoped<IDateTime, DateTimeService>()
                   .AddScoped<IExcelService, ExcelService>()
                   .AddScoped<IUploadService, UploadService>()
                   .AddScoped<IDocumentOcrJob, DocumentOcrJob>();
}
