namespace CleanArchitecture.Blazor.Application.Features.Invoices.Caching;
/// <summary>
/// Static class for managing cache keys and expiration for Invoice-related data.
/// </summary>
public static class InvoiceCacheKey
{
    public const string GetAllCacheKey = "all-Invoices";
    public static string GetPaginationCacheKey(string parameters)
    {
        return $"InvoiceCacheKey:InvoicesWithPaginationQuery,{parameters}";
    }
    public static string GetExportCacheKey(string parameters)
    {
        return $"InvoiceCacheKey:ExportCacheKey,{parameters}";
    }
    public static string GetByNameCacheKey(string parameters)
    {
        return $"InvoiceCacheKey:GetByNameCacheKey,{parameters}";
    }
    public static string GetByIdCacheKey(string parameters)
    {
        return $"InvoiceCacheKey:GetByIdCacheKey,{parameters}";
    }
    public static IEnumerable<string>? Tags => new string[] { "invoice" };
    public static void Refresh()
    {
        FusionCacheFactory.RemoveByTags(Tags);
    }
}

