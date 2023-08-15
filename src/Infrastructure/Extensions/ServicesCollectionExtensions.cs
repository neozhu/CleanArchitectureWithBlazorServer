using System.Runtime.Versioning;
using CleanArchitecture.Blazor.Application.Services.PaddleOCR;
using CleanArchitecture.Blazor.Infrastructure.Services.PaddleOCR;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

[SupportedOSPlatform("windows")]
public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<ExceptionHandlingMiddleware>()
            .AddScoped<IDateTime, DateTimeService>()
            .AddScoped<IExcelService, ExcelService>()
            .AddScoped<IUploadService, UploadService>()
            .AddTransient<IDocumentOcrJob, DocumentOcrJob>()
            .AddScoped<IPDFService, PDFService>();
    }
}