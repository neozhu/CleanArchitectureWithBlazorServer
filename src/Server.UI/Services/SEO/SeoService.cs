using CleanArchitecture.Blazor.Server.UI.Models.SEO;

namespace CleanArchitecture.Blazor.Server.UI.Services.SEO;

/// <summary>
/// Implementation of SEO service for managing metadata and structured data
/// </summary>
public class SeoService : ISeoService
{
    private readonly Dictionary<string, PageSeoData> _pageSeoData;

    public SeoService()
    {
        _pageSeoData = InitializePageSeoData();
    }

    /// <summary>
    /// Initialize predefined SEO data for common pages
    /// </summary>
    /// <returns>Dictionary of page SEO data</returns>
    private Dictionary<string, PageSeoData> InitializePageSeoData()
    {
        return new Dictionary<string, PageSeoData>(StringComparer.OrdinalIgnoreCase)
        {
            { "dashboard", PageSeoDataDefaults.Dashboard },
            { "home", PageSeoDataDefaults.Dashboard },
            { "contacts", PageSeoDataDefaults.Contacts },
            { "products", PageSeoDataDefaults.Products },
            { "users", PageSeoDataDefaults.UserManagement },
            { "user-management", PageSeoDataDefaults.UserManagement },
            { "documents", PageSeoDataDefaults.Documents },
            { "login", PageSeoDataDefaults.Login },
            { "register", PageSeoDataDefaults.Register },
            { "sign-up", PageSeoDataDefaults.Register }
        };
    }

    /// <inheritdoc />
    public PageSeoData GetPageSeoData(string pageName)
    {
        if (string.IsNullOrWhiteSpace(pageName))
        {
            return CreateDefaultSeoData();
        }

        // Try to find exact match first
        if (_pageSeoData.TryGetValue(pageName, out var seoData))
        {
            return seoData;
        }

        // Try to find partial match for complex page paths
        var key = _pageSeoData.Keys.FirstOrDefault(k => 
            pageName.Contains(k, StringComparison.OrdinalIgnoreCase) ||
            k.Contains(pageName, StringComparison.OrdinalIgnoreCase));

        if (key != null && _pageSeoData.TryGetValue(key, out var partialMatch))
        {
            return partialMatch;
        }

        // Return default SEO data if no match found
        return CreateDefaultSeoData(pageName);
    }

    /// <inheritdoc />
    public object? GenerateStructuredData(string pageName, object? additionalData = null)
    {
        var seoData = GetPageSeoData(pageName);
        
        if (!seoData.EnableJsonLd)
        {
            return null;
        }

        // Return existing JSON-LD data if available
        if (seoData.JsonLdData != null)
        {
            return seoData.JsonLdData;
        }

        // Generate default structured data based on page type
        return pageName.ToLowerInvariant() switch
        {
            "dashboard" or "home" => GenerateWebApplicationStructuredData(),
            "contacts" => GenerateContactsPageStructuredData(),
            "products" => GenerateProductsPageStructuredData(),
            "users" or "user-management" => GenerateUserManagementStructuredData(),
            "documents" => GenerateDocumentsPageStructuredData(),
            _ => GenerateWebPageStructuredData(seoData)
        };
    }

    /// <inheritdoc />
    public object GenerateBreadcrumbStructuredData(List<BreadcrumbItem> breadcrumbs)
    {
        return new
        {
            context = "https://schema.org",
            type = "BreadcrumbList",
            itemListElement = breadcrumbs.Select(b => new
            {
                type = "ListItem",
                position = b.Position,
                name = b.Name,
                item = b.Url
            }).ToArray()
        };
    }

    /// <inheritdoc />
    public object GenerateOrganizationStructuredData()
    {
        return new
        {
            context = "https://schema.org",
            type = "Organization",
            name = SeoSettings.DefaultSiteName,
            description = SeoSettings.DefaultDescription,
            url = "https://architecture.blazorserver.com",
            logo = new
            {
                type = "ImageObject",
                                    url = "https://architecture.blazorserver.com/img/blazorstudio.png"
            },
            contactPoint = new
            {
                type = "ContactPoint",
                contactType = "customer service",
                availableLanguage = new[] { "en", "zh" }
            }
        };
    }

    /// <summary>
    /// Create default SEO data for pages without specific configuration
    /// </summary>
    /// <param name="pageName">Optional page name for title generation</param>
    /// <returns>Default PageSeoData</returns>
    private PageSeoData CreateDefaultSeoData(string? pageName = null)
    {
        var title = !string.IsNullOrWhiteSpace(pageName) 
            ? $"{FormatPageName(pageName)} - {SeoSettings.DefaultSiteName}"
            : SeoSettings.DefaultSiteName;

        return new PageSeoData
        {
            Title = title,
            Description = SeoSettings.DefaultDescription,
            Keywords = SeoSettings.DefaultKeywords,
            ImageUrl = SeoSettings.DefaultImage,
            ImageAlt = SeoSettings.DefaultImageAlt,
            Type = "website"
        };
    }

    /// <summary>
    /// Format page name for display
    /// </summary>
    /// <param name="pageName">Raw page name</param>
    /// <returns>Formatted page name</returns>
    private string FormatPageName(string pageName)
    {
        return pageName
            .Replace("-", " ")
            .Replace("_", " ")
            .Split(' ')
            .Select(word => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant())
            .Aggregate((a, b) => $"{a} {b}");
    }

    /// <summary>
    /// Generate structured data for web application
    /// </summary>
    /// <returns>WebApplication structured data</returns>
    private object GenerateWebApplicationStructuredData()
    {
        return new
        {
            context = "https://schema.org",
            type = "WebApplication",
            name = SeoSettings.DefaultSiteName,
            description = SeoSettings.DefaultDescription,
            url = "https://architecture.blazorserver.com",
            applicationCategory = "BusinessApplication",
            operatingSystem = "Web Browser",
            offers = new
            {
                type = "Offer",
                price = "0",
                priceCurrency = "USD"
            },
            featureList = new[]
            {
                "Clean Architecture Implementation",
                "User Management System",
                "Document Management",
                "Contact Management",
                "Product Catalog",
                "Real-time Dashboard",
                "Multi-language Support",
                "Role-based Access Control"
            }
        };
    }

    /// <summary>
    /// Generate structured data for contacts page
    /// </summary>
    /// <returns>WebPage structured data for contacts</returns>
    private object GenerateContactsPageStructuredData()
    {
        return new
        {
            context = "https://schema.org",
            type = "WebPage",
            name = "Contact Management",
            description = "Manage your contacts efficiently with advanced contact management system",
            url = "https://architecture.blazorserver.com/pages/contacts",
            mainEntity = new
            {
                type = "SoftwareApplication",
                name = "Contact Management System",
                applicationCategory = "BusinessApplication"
            }
        };
    }

    /// <summary>
    /// Generate structured data for products page
    /// </summary>
    /// <returns>WebPage structured data for products</returns>
    private object GenerateProductsPageStructuredData()
    {
        return new
        {
            context = "https://schema.org",
            type = "WebPage",
            name = "Product Management",
            description = "Comprehensive product management system with inventory tracking",
            url = "https://architecture.blazorserver.com/pages/products",
            mainEntity = new
            {
                type = "SoftwareApplication",
                name = "Product Management System",
                applicationCategory = "BusinessApplication"
            }
        };
    }

    /// <summary>
    /// Generate structured data for user management page
    /// </summary>
    /// <returns>WebPage structured data for user management</returns>
    private object GenerateUserManagementStructuredData()
    {
        return new
        {
            context = "https://schema.org",
            type = "WebPage",
            name = "User Management",
            description = "Advanced user management with role-based access control",
            url = "https://architecture.blazorserver.com/pages/identity/users",
            mainEntity = new
            {
                type = "SoftwareApplication",
                name = "User Management System",
                applicationCategory = "SecurityApplication"
            }
        };
    }

    /// <summary>
    /// Generate structured data for documents page
    /// </summary>
    /// <returns>WebPage structured data for documents</returns>
    private object GenerateDocumentsPageStructuredData()
    {
        return new
        {
            context = "https://schema.org",
            type = "WebPage",
            name = "Document Management",
            description = "Secure document management with file organization capabilities",
            url = "https://architecture.blazorserver.com/pages/documents",
            mainEntity = new
            {
                type = "SoftwareApplication",
                name = "Document Management System",
                applicationCategory = "BusinessApplication"
            }
        };
    }

    /// <summary>
    /// Generate generic web page structured data
    /// </summary>
    /// <param name="seoData">SEO data for the page</param>
    /// <returns>WebPage structured data</returns>
    private object GenerateWebPageStructuredData(PageSeoData seoData)
    {
        return new
        {
            context = "https://schema.org",
            type = "WebPage",
            name = seoData.Title ?? SeoSettings.DefaultSiteName,
            description = seoData.Description ?? SeoSettings.DefaultDescription,
            mainEntity = new
            {
                type = "WebSite",
                name = SeoSettings.DefaultSiteName,
                description = SeoSettings.DefaultDescription
            }
        };
    }
}
