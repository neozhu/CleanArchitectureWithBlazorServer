using System;
using System.Collections.Generic;
using System.Linq;

namespace Common;
public static class EnumExtensions
{
    public static TEnum? MaxEnumValue<TEnum>(this IEnumerable<string> enumStringValues) where TEnum : struct
    {
        if (!typeof(TEnum).IsEnum)
        {
            throw new ArgumentException($"{nameof(TEnum)} must be an enumerated type");
        }

        var enumValues = enumStringValues
            .Select(enumStr => Enum.TryParse<TEnum>(enumStr, out var enumValue) ? enumValue : (TEnum?)null)
            .Where(enumValue => enumValue.HasValue)
            .Select(enumValue => enumValue.Value);

        return enumValues.Any() ? enumValues.Max() : (TEnum?)null;
    }

    public static string MaxEnumString<TEnum>(this IEnumerable<string>? enumStringValues) where TEnum : struct
    {
        if (!typeof(TEnum).IsEnum)
        {
            throw new ArgumentException($"{nameof(TEnum)} must be an enumerated type");
        }
        if (enumStringValues == null || !enumStringValues.Any()) return string.Empty;

        var enumValues = enumStringValues
            .Where(enumStr => Enum.TryParse<TEnum>(enumStr, out _))
            .DefaultIfEmpty()
            .Max();

        return enumValues ?? "No valid enum values";
    }

}
