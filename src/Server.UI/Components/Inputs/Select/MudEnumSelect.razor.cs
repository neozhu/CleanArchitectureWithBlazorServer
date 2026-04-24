using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Components.Rendering;

namespace CleanArchitecture.Blazor.Server.UI.Components.Inputs.Select;

/// <summary>
/// A generic MudSelect component for Enums that automatically generates options 
/// based on [Display] or [Description] attributes.
/// </summary>
/// <typeparam name="TEnum">The enum type.</typeparam>
public class MudEnumSelect<TEnum> : MudSelect<TEnum>
{
    // ----- Static Metadata Cache -----

    // Handle nullable enums (e.g., Status?) to get the underlying type (Status)
    private static readonly Type UnderlyingEnumType = Nullable.GetUnderlyingType(typeof(TEnum)) ?? typeof(TEnum);

    private static readonly bool IsFlags = UnderlyingEnumType.IsDefined(typeof(FlagsAttribute), false);

    // Cache enum values to avoid expensive Enum.GetValues() calls during rendering
    private static readonly IReadOnlyList<TEnum> EnumValues = Enum.GetValues(UnderlyingEnumType).Cast<TEnum>().ToArray();

    // Cache the strategy to retrieve display text. 
    // We cache Func<string> instead of string to support runtime localization (changing culture on the fly).
    private static readonly Dictionary<object, Func<string>> DisplayFuncs = InitializeDisplayFuncs();

    /// <summary>
    /// Constructor.
    /// </summary>
    public MudEnumSelect()
    {
        // Optimization: Set defaults in the constructor so they can be overridden by Razor parameters.
        // If these were in OnInitialized, they would overwrite user settings like <MudEnumSelect Dense="false" />.
        Dense = true;
        TransformOrigin = Origin.TopLeft;
        AnchorOrigin = Origin.BottomLeft;

        // Automatically enable MultiSelection if the Enum has the [Flags] attribute.
        if (IsFlags)
        {
            MultiSelection = true;
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Set the ToStringFunc to use our cached display logic.
        // Only set if the user hasn't provided a custom one.
        if (ToStringFunc == null)
        {
            ToStringFunc = v => GetDisplayText(v);
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // If the user did not provide manual <MudSelectItem> content, generate it automatically.
        if (ChildContent == null)
        {
            ChildContent = BuildEnumOptions;
        }
    }

    /// <summary>
    /// Renders the MudSelectItem components for each enum value.
    /// </summary>
    private void BuildEnumOptions(RenderTreeBuilder builder)
    {
        foreach (var item in EnumValues)
        {
            // We use the item hash code or a counter as the sequence number mechanism relies on order.
            // Here we let the builder handle sequences naturally by calling OpenComponent.
            builder.OpenComponent<MudSelectItem<TEnum>>(0);

            // Set the Value parameter
            builder.AddAttribute(1, nameof(MudSelectItem<TEnum>.Value), item);

            // Get the display text
            string text = GetDisplayText(item);

            // Render the text inside the Item's ChildContent. 
            // This ensures the item displays correctly in the dropdown list.
            builder.AddAttribute(2, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, text)));

            builder.CloseComponent();
        }
    }

    // ----- Helpers -----

    /// <summary>
    /// Retrieves the display text for a specific enum value using the cached strategy.
    /// </summary>
    private static string GetDisplayText(TEnum? value)
    {
        if (value is null) return string.Empty;

        // Try to get the cached function
        if (DisplayFuncs.TryGetValue(value, out var func))
        {
            return func();
        }

        // Fallback
        return value.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Scans the Enum type once to determine how to retrieve names (DisplayAttribute vs DescriptionAttribute vs ToString).
    /// </summary>
    private static Dictionary<object, Func<string>> InitializeDisplayFuncs()
    {
        var result = new Dictionary<object, Func<string>>();

        foreach (var value in EnumValues)
        {
            if (value is null) continue;

            var name = value.ToString();
            // Get the field info for the enum value
            var field = UnderlyingEnumType.GetField(name!);

            if (field != null)
            {
                // Priority 1: [Display(Name = "...", ResourceType = ...)]
                // This supports standard .NET localization.
                var displayAttr = field.GetCustomAttribute<DisplayAttribute>();
                if (displayAttr != null)
                {
                    // We capture the attribute instance. GetName() checks the ResourceType at runtime.
                    result[value] = () => displayAttr.GetName() ?? name!;
                    continue;
                }

                // Priority 2: [Description("...")]
                // Legacy support for System.ComponentModel.
                var descAttr = field.GetCustomAttribute<DescriptionAttribute>();
                if (descAttr != null && !string.IsNullOrWhiteSpace(descAttr.Description))
                {
                    result[value] = () => descAttr.Description;
                    continue;
                }
            }

            // Priority 3: Default ToString()
            result[value] = () => value.ToString()!;
        }

        return result;
    }
}
