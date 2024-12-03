
namespace CleanArchitecture.Blazor.Application.Features.Categories.Caching;

public static class CategoryCacheKey
{
    public const string GetAllCacheKey = "all-Categories";
    public static string GetPaginationCacheKey(string parameters) {
        return $"CategoryCacheKey:CategoriesWithPaginationQuery,{parameters}";
    }
    public static string GetExportCacheKey(string parameters) {
        return $"CategoryCacheKey:ExportCacheKey,{parameters}";
    }
    public static string GetByNameCacheKey(string parameters) {
        return $"CategoryCacheKey:GetByNameCacheKey,{parameters}";
    }
    public static string GetByIdCacheKey(string parameters) {
        return $"CategoryCacheKey:GetByIdCacheKey,{parameters}";
    }
    public static IEnumerable<string>? Tags => new string[] { "category" };
    public static void Refresh()
    {
        FusionCacheFactory.RemoveByTags(Tags);
    }
}

