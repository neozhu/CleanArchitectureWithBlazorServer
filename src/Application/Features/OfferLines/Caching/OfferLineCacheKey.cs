

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Caching;
/// <summary>
/// Static class for managing cache keys and expiration for OfferLine-related data.
/// </summary>
public static class OfferLineCacheKey
{
    public const string GetAllCacheKey = "all-OfferLines";
    public static string GetPaginationCacheKey(string parameters) {
        return $"OfferLineCacheKey:OfferLinesWithPaginationQuery,{parameters}";
    }
    public static string GetExportCacheKey(string parameters) {
        return $"OfferLineCacheKey:ExportCacheKey,{parameters}";
    }
    public static string GetByNameCacheKey(string parameters) {
        return $"OfferLineCacheKey:GetByNameCacheKey,{parameters}";
    }
    public static string GetByIdCacheKey(string parameters) {
        return $"OfferLineCacheKey:GetByIdCacheKey,{parameters}";
    }
    public static IEnumerable<string>? Tags => new string[] { "offerline" };
    public static void Refresh()
    {
        FusionCacheFactory.RemoveByTags(Tags);
    }
}

