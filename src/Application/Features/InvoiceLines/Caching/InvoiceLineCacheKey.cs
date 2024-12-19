

namespace CleanArchitecture.Blazor.Application.Features.InvoiceLines.Caching;
/// <summary>
/// Static class for managing cache keys and expiration for InvoiceLine-related data.
/// </summary>
public static class InvoiceLineCacheKey
{
    public const string GetAllCacheKey = "all-InvoiceLines";
    public static string GetPaginationCacheKey(string parameters) {
        return $"InvoiceLineCacheKey:InvoiceLinesWithPaginationQuery,{parameters}";
    }
    public static string GetExportCacheKey(string parameters) {
        return $"InvoiceLineCacheKey:ExportCacheKey,{parameters}";
    }
    public static string GetByNameCacheKey(string parameters) {
        return $"InvoiceLineCacheKey:GetByNameCacheKey,{parameters}";
    }
    public static string GetByIdCacheKey(string parameters) {
        return $"InvoiceLineCacheKey:GetByIdCacheKey,{parameters}";
    }
    public static IEnumerable<string>? Tags => new string[] { "invoiceline" };
    public static void Refresh()
    {
        FusionCacheFactory.RemoveByTags(Tags);
    }
}

