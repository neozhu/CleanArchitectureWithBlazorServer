using CleanArchitecture.Blazor.Application.Services.PaddleOCR;
using CleanArchitecture.Blazor.Infrastructure.Services.PaddleOCR;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
        => services.AddSingleton<ICurrentUserService, CurrentUserService>()
                    .AddSingleton<ExceptionHandlingMiddleware>()
                   .AddScoped<IDateTime, DateTimeService>()
                   .AddScoped<IExcelService, ExcelService>()
                   .AddScoped<IUploadService, UploadService>()
                   .AddScoped<IDocumentOcrJob, DocumentOcrJob>()
                   .AddScoped<IPDFService, PDFService>();
          
}
