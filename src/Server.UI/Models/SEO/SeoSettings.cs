namespace CleanArchitecture.Blazor.Server.UI.Models.SEO;

/// <summary>
/// SEO configuration settings for the application
/// </summary>
public static class SeoSettings
{
    /// <summary>
    /// Default site name
    /// </summary>
    public const string DefaultSiteName = "Clean Architecture With Blazor Server";
    
    /// <summary>
    /// Default site description
    /// </summary>
    public const string DefaultDescription = "Enterprise-ready Blazor Server application template built with Clean Architecture principles, featuring comprehensive dashboard, user management, and modern web technologies.";
    
    /// <summary>
    /// Default author
    /// </summary>
    public const string DefaultAuthor = "Blazor Studio";
    
    /// <summary>
    /// Default keywords
    /// </summary>
    public const string DefaultKeywords = "Blazor Server, Clean Architecture, Dashboard, .NET, Web Application, Enterprise, Template, MudBlazor, CQRS, Authentication";
    
    /// <summary>
    /// Default image for social sharing
    /// </summary>
    public const string DefaultImage = "img/blazorstudio.png";
    
    /// <summary>
    /// Default image alt text
    /// </summary>
    public const string DefaultImageAlt = "Clean Architecture Blazor Server Dashboard";
    
    /// <summary>
    /// Default locale
    /// </summary>
    public const string DefaultLocale = "en_US";
    
    /// <summary>
    /// Default robots directive
    /// </summary>
    public const string DefaultRobots = "index, follow";
}

/// <summary>
/// Page-specific SEO metadata
/// </summary>
public class PageSeoData
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Keywords { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAlt { get; set; }
    public string? Type { get; set; } = "website";
    public bool NoIndex { get; set; } = false;
    public string? CanonicalUrl { get; set; }
    public Dictionary<string, string>? CustomMeta { get; set; }
    public object? JsonLdData { get; set; }
    public bool EnableJsonLd { get; set; } = true;
}

/// <summary>
/// Predefined SEO data for common pages
/// </summary>
public static class PageSeoDataDefaults
{
    /// <summary>
    /// SEO data for the home/dashboard page
    /// </summary>
    public static PageSeoData Dashboard => new()
    {
        Title = "Dashboard - Clean Architecture Blazor Server",
        Description = "Enterprise dashboard built with Blazor Server and Clean Architecture. Monitor your application metrics, user activities, and system health in real-time.",
        Keywords = "Dashboard, Analytics, Blazor Server, Real-time, Metrics, Enterprise",
        Type = "website",
        JsonLdData = new
        {
            context = "https://schema.org",
            type = "WebApplication",
            name = SeoSettings.DefaultSiteName,
            description = SeoSettings.DefaultDescription,
            url = "https://architecture.blazorserver.com",
            applicationCategory = "BusinessApplication",
            operatingSystem = "Web Browser"
        }
    };

    /// <summary>
    /// SEO data for contacts page
    /// </summary>
    public static PageSeoData Contacts => new()
    {
        Title = "Contacts Management - Clean Architecture Blazor Server",
        Description = "Manage your contacts efficiently with our advanced contact management system. Add, edit, search, and organize contacts with powerful filtering and sorting capabilities.",
        Keywords = "Contacts, CRM, Management, Blazor, Address Book, Contact List",
        Type = "website"
    };

    /// <summary>
    /// SEO data for products page
    /// </summary>
    public static PageSeoData Products => new()
    {
        Title = "Products Management - Clean Architecture Blazor Server",
        Description = "Comprehensive product management system with inventory tracking, categorization, and advanced search capabilities. Built with Blazor Server and Clean Architecture.",
        Keywords = "Products, Inventory, Management, E-commerce, Catalog, Blazor",
        Type = "website"
    };

    /// <summary>
    /// SEO data for user management pages
    /// </summary>
    public static PageSeoData UserManagement => new()
    {
        Title = "User Management - Clean Architecture Blazor Server",
        Description = "Advanced user management system with role-based access control, user profiles, and comprehensive security features built on ASP.NET Core Identity.",
        Keywords = "User Management, Security, Authentication, Authorization, Identity, Roles",
        Type = "website"
    };

    /// <summary>
    /// SEO data for documents page
    /// </summary>
    public static PageSeoData Documents => new()
    {
        Title = "Document Management - Clean Architecture Blazor Server",
        Description = "Secure document management system with file upload, organization, and sharing capabilities. Built with modern web technologies and security best practices.",
        Keywords = "Documents, File Management, Upload, Storage, Security, Organization",
        Type = "website"
    };

    /// <summary>
    /// SEO data for login page
    /// </summary>
    public static PageSeoData Login => new()
    {
        Title = "Sign In - Clean Architecture Blazor Server Demo",
        Description = "Sign in to explore the Clean Architecture Blazor Server demo application. Experience modern web development with .NET Blazor, MudBlazor UI, and enterprise-grade features.",
        Keywords = "Blazor Server Login, Demo Application, Clean Architecture, .NET Authentication, MudBlazor UI, Web Development Showcase",
        Type = "website",
        NoIndex = false // Allow indexing for demo purposes
    };

    /// <summary>
    /// SEO data for registration page
    /// </summary>
    public static PageSeoData Register => new()
    {
        Title = "Register - Clean Architecture Blazor Server",
        Description = "Create your account to access the Clean Architecture Blazor Server dashboard. Join thousands of users using our enterprise-ready platform.",
        Keywords = "Register, Sign Up, Account, Create Account, Join",
        Type = "website",
        NoIndex = true // Don't index registration pages
    };
}
