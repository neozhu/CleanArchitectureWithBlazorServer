

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Caching;
/// <summary>
/// Static class for managing cache keys and expiration for SupplyItem-related data.
/// </summary>
public static class SupplyItemCacheKey
{
    public const string GetAllCacheKey = "all-SupplyItems";
    public static string GetPaginationCacheKey(string parameters) {
        return $"SupplyItemCacheKey:SupplyItemsWithPaginationQuery,{parameters}";
    }
    public static string GetExportCacheKey(string parameters) {
        return $"SupplyItemCacheKey:ExportCacheKey,{parameters}";
    }
    public static string GetByNameCacheKey(string parameters) {
        return $"SupplyItemCacheKey:GetByNameCacheKey,{parameters}";
    }
    public static string GetByIdCacheKey(string parameters) {
        return $"SupplyItemCacheKey:GetByIdCacheKey,{parameters}";
    }
    public static IEnumerable<string>? Tags => new string[] { "supplyitem" };
    public static void Refresh()
    {
        FusionCacheFactory.RemoveByTags(Tags);
    }
}

