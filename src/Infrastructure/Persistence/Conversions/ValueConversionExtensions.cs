using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Conversions;
#nullable disable warnings
public static class ValueConversionExtensions
{
    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
    {
         

        var converter = new ValueConverter<T, string>(
            v => JsonSerializer.Serialize(v, JsonSerializerOptions.Web),
            v => string.IsNullOrEmpty(v) ? default : JsonSerializer.Deserialize<T>(v, JsonSerializerOptions.Web));

        var comparer = new ValueComparer<T>(
            (l, r) => JsonSerializer.Serialize(l, JsonSerializerOptions.Web) == JsonSerializer.Serialize(r, JsonSerializerOptions.Web),
            v => v == null ? 0 : JsonSerializer.Serialize(v, JsonSerializerOptions.Web).GetHashCode(),
            v => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(v, JsonSerializerOptions.Web), JsonSerializerOptions.Web));

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);

        return propertyBuilder;
    }
}
