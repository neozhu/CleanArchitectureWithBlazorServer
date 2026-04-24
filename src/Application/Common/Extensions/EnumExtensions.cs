using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Application.Common.Extensions;

public static class EnumExtensions
{
  
    private static readonly ConcurrentDictionary<Enum, string> _cache = new();

    public static string GetDisplayName(this Enum value)
    {
      
        return _cache.GetOrAdd(value, (enumVal) =>
        {
            var type = enumVal.GetType();
            var name = Enum.GetName(type, enumVal);
            if (name == null) return enumVal.ToString();

            var field = type.GetField(name);
            if (field == null) return name;

            var attribute = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute;
            return attribute?.GetName() ?? name;
        });
    }
}
