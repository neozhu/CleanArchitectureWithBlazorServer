using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;
using CleanArchitecture.Blazor.Infrastructure.Services.PaddleOCR;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<PicklistService>();
        services.AddSingleton<IPicklistService>(sp =>
        {
            var service = sp.GetRequiredService<PicklistService>();
            service.Initialize();
            return service;
        });

        services.AddSingleton<TenantService>();
        services.AddSingleton<ITenantService>(sp =>
        {
            var service = sp.GetRequiredService<TenantService>();
            service.Initialize();
            return service;
        });

        services.AddScoped<IValidationService, ValidationService>();

        return services
            .AddScoped<IDateTime, DateTimeService>()
            .AddScoped<IExcelService, ExcelService>()
            .AddScoped<IUploadService, UploadService>()
            .AddTransient<IDocumentOcrJob, DocumentOcrJob>()
            .AddScoped<IPDFService, PDFService>();
    }
}