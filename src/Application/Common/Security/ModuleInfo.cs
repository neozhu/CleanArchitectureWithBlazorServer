namespace CleanArchitecture.Blazor.Application.Common.Security;

/// <summary>
/// Contains information about a permission module.
/// </summary>
public class ModuleInfo
{
    /// <summary>
    /// Gets the display name of the module.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets the description of the module.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleInfo"/> class.
    /// </summary>
    /// <param name="displayName">Display name of the module.</param>
    /// <param name="description">Description of the module.</param>
    public ModuleInfo(string displayName, string description)
    {
        DisplayName = displayName;
        Description = description;
    }
} 
