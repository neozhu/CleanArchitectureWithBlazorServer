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

    // var sortedList = EnumSorting.SortByEnum<string, MyEnum>(stringList);
    public static List<CustomType> SortByEnum<CustomType, enumType>(IEnumerable<CustomType?> itemList, bool descending = false) where enumType : IComparable
    {
        itemList = itemList.Distinct().ToList();
        //below extracts all enum values
        var itemToEnumMap = Enum.GetValues(typeof(enumType))
                                 .Cast<enumType>()
                                 .ToDictionary(enumValue => enumValue.ToString());

        //then sort by value
        if (descending)
            return itemList.OrderByDescending(item => itemToEnumMap[item.ToString()]).ToList();
        return itemList.OrderBy(item => itemToEnumMap[item.ToString()]).ToList();
    }

    public static Enum? MaxEnum<TEnum>(this IEnumerable<Enum>? enumValues) where TEnum : struct
    {
        if (!typeof(TEnum).IsEnum)
        {
            throw new ArgumentException($"{nameof(TEnum)} must be an enumerated type");
        }
        if (enumValues == null || !enumValues.Any()) return null;

        return enumValues.OrderBy(e => Convert.ToByte(e)).Last();//ascending order
    }

}
