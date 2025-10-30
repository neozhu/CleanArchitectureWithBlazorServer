using System.ComponentModel;

namespace CleanArchitecture.Blazor.Server.UI.Components.Forms;

public partial class MudEnumSelect<TEnum> : MudSelect<TEnum>
{
    // ----- Static metadata -----
    private static readonly Type UnderlyingEnumType = Nullable.GetUnderlyingType(typeof(TEnum)) ?? typeof(TEnum);
    private static readonly bool IsFlags = UnderlyingEnumType.IsDefined(typeof(FlagsAttribute), false);
    private static readonly bool IsNullable = Nullable.GetUnderlyingType(typeof(TEnum)) is not null;
    private static readonly IReadOnlyList<TEnum> EnumValues = UnderlyingEnumType.GetEnumValues().Cast<TEnum>().ToArray();
    // Use object as key to tolerate nullable enum generic instantiations.
    private static readonly Dictionary<object, string> DisplayCache = EnumValues
        .Where(v => v is not null)
        .ToDictionary(v => (object)v!, v => BuildDisplayText(v!));

    private bool _optionsInitialized;

    protected override void OnInitialized()
    {
        // Configure base select behavior.
        Dense = true;
        TransformOrigin = Origin.TopLeft;
        AnchorOrigin = Origin.BottomLeft;
        MultiSelection = IsFlags;
        // Ensure selected value uses description text.
        ToStringFunc = v => v is null ? string.Empty : BuildDisplayText(v);
        base.OnInitialized();
        if (!_optionsInitialized)
            BuildOptions();
    }


    public override async Task SetParametersAsync(ParameterView parameters)
    {
        // Keep this minimal: only apply parameters.
        await base.SetParametersAsync(parameters);
    }

    protected override void OnParametersSet()
    {
      
        SyncSelectedFromValue();
        base.OnParametersSet();
    }

    private void BuildOptions()
    {
        if (ChildContent != null)
        {
            _optionsInitialized = true;
            return;
        }

        ChildContent = builder =>
        {
            // Dynamic sequence numbers are required here because the number of enum values is not known at compile time.
            // Suppress analyzer warning ASP0006 for this dynamic region.
#pragma warning disable ASP0006
            var seq = 0;
            foreach (var item in EnumValues)
            {
                builder.OpenComponent<MudSelectItem<TEnum>>(seq++);
                builder.AddAttribute(seq++, "Value", item);
                var key = (object)item!;
                var text = DisplayCache.TryGetValue(key, out var d) ? d : item?.ToString() ?? string.Empty;
                // Render via ChildContent to guarantee proper item text.
                builder.AddAttribute(seq++, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, text)));
                builder.CloseComponent();
            }
#pragma warning restore ASP0006
        };
        _optionsInitialized = true;
    }

    private void SyncSelectedFromValue()
    {
        if (Value is null)
        {
            SelectedValues = Array.Empty<TEnum>();
            return;
        }

        if (IsFlags)
        {
            if (Value is Enum raw)
            {
                SelectedValues = EnumValues.Where(v => raw.HasFlag((Enum)(object)v!)).ToList();
            }
            else
            {
                SelectedValues = Array.Empty<TEnum>();
            }
        }
        else
        {
            SelectedValues = new[] { Value };
        }
    }

    // Build display text using DescriptionAttribute (if present) else enum name.
    private static string BuildDisplayText(TEnum value)
    {
        if (value is null) return string.Empty;
        if (value is Enum e)
        {
            var name = e.ToString();
            var members = e.GetType().GetMember(name);
            if (members.Length > 0)
            {
                var member = members[0];
                var descriptionAttributes = member.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (descriptionAttributes.Length > 0)
                {
                    var description = ((DescriptionAttribute)descriptionAttributes[0]).Description;
                    if (!string.IsNullOrWhiteSpace(description)) return description;
                }
            }
            return name;
        }
        return value.ToString() ?? string.Empty;
    }

    // (Optional future use) Aggregate flags from selected values.
    private static ulong ToUInt64(Enum e) => Convert.ToUInt64(e);
    private static TEnum FromUInt64(ulong raw) => (TEnum)Enum.ToObject(UnderlyingEnumType, raw);

    private static TEnum AggregateFlags(IEnumerable<TEnum> items)
    {
        ulong acc = 0;
        foreach (var i in items)
        {
            if (i is null) continue;
            acc |= ToUInt64((Enum)(object)i!);
        }
        return FromUInt64(acc);
    }
}
