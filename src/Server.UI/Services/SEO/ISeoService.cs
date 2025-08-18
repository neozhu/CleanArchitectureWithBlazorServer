using CleanArchitecture.Blazor.Server.UI.Models.SEO;

namespace CleanArchitecture.Blazor.Server.UI.Services.SEO;

/// <summary>
/// Service for managing SEO metadata and structured data
/// </summary>
public interface ISeoService
{
    /// <summary>
    /// Get SEO data for a specific page
    /// </summary>
    /// <param name="pageName">The name/identifier of the page</param>
    /// <returns>PageSeoData for the specified page</returns>
    PageSeoData GetPageSeoData(string pageName);
    
    /// <summary>
    /// Generate structured data for a specific page
    /// </summary>
    /// <param name="pageName">The name/identifier of the page</param>
    /// <param name="additionalData">Additional data to include in structured data</param>
    /// <returns>Structured data object</returns>
    object? GenerateStructuredData(string pageName, object? additionalData = null);
    
    /// <summary>
    /// Generate breadcrumb structured data
    /// </summary>
    /// <param name="breadcrumbs">List of breadcrumb items</param>
    /// <returns>Breadcrumb structured data</returns>
    object GenerateBreadcrumbStructuredData(List<BreadcrumbItem> breadcrumbs);
    
    /// <summary>
    /// Generate organization structured data
    /// </summary>
    /// <returns>Organization structured data</returns>
    object GenerateOrganizationStructuredData();
}

/// <summary>
/// Breadcrumb item for structured data
/// </summary>
public class BreadcrumbItem
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Position { get; set; }
}
