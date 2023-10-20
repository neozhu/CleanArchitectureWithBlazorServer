using System.ComponentModel;
using System.Reflection;

namespace CleanArchitecture.Blazor.Server.UI.Components.Common;
public partial class MudEnumSelect<TEnum> : MudSelect<TEnum>
{
    private bool _isFlagsEnum;
    private static bool IsNullableEnum => Nullable.GetUnderlyingType(typeof(TEnum))?.IsEnum ?? false;

    private static IList<TEnum> EnumValueList => EnumOrUnderlyingType!.GetEnumValues().Cast<TEnum>().ToList();

    private static Type? EnumOrUnderlyingType => IsNullableEnum ? Nullable.GetUnderlyingType(typeof(TEnum)) : typeof(TEnum);

    protected override void OnInitialized()
    {
        Dense = true;
        TransformOrigin = Origin.TopLeft;
        AnchorOrigin = Origin.BottomLeft;
        _isFlagsEnum = EnumOrUnderlyingType!.GetCustomAttribute<FlagsAttribute>() != null;
        MultiSelection = _isFlagsEnum;
        base.OnInitialized();
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);
        Setup();
        setSelectedValues();
    }
    private void setSelectedValues()
    {
        if (_isFlagsEnum)
        {
            var valueEnum = Value as Enum;
            var toSelect = EnumValueList.Where(e => valueEnum?.HasFlag((e as Enum)!) == true).ToList();
            SelectedValues = toSelect;
        }
        else if (Value != null)
        {
            SelectedValues = new[] { Value };
        }
    }

    private string GetDescriptionText<T>(T val)
    {
        if (val is null) return string.Empty;
        var attributes = (DescriptionAttribute[])val.GetType().GetField(val!.ToString()!)!.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : val!.ToString()!;
    }

    private T SetFlag<T>(T flag, bool set)
    {
        Enum? value = Value as Enum;

        Type underlyingType = Enum.GetUnderlyingType(EnumOrUnderlyingType!);

        // note: AsInt mean: math integer vs enum (not the c# int type)
        dynamic valueAsInt = Convert.ChangeType(value!, underlyingType);
        dynamic flagAsInt = Convert.ChangeType(flag!, underlyingType);
        if (set)
            valueAsInt |= flagAsInt;
        else
            valueAsInt &= ~flagAsInt;

        return (T)valueAsInt;
    }
}
