using System.Text.Json;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Conversions;
public static class ValueConversionExtensions
{
    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
    {
        ValueConverter<T, String> converter = new ValueConverter<T, String>(
            v => JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options),
            v => string.IsNullOrEmpty(v)? default: JsonSerializer.Deserialize<T>(v, DefaultJsonSerializerOptions.Options));

        ValueComparer<T> comparer = new ValueComparer<T>(
            (l, r) => JsonSerializer.Serialize(l, DefaultJsonSerializerOptions.Options) == JsonSerializer.Serialize(r, DefaultJsonSerializerOptions.Options),
            v => v == null ? 0 : JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options).GetHashCode(),
            v => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options), DefaultJsonSerializerOptions.Options));

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueConverter(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);
        return propertyBuilder;
    }

    public static PropertyBuilder<List<string>> HasStringListConversion(this PropertyBuilder<List<string>> propertyBuilder)
    {
        ValueConverter<List<string>, String> converter = new ValueConverter<List<string>, String>(
           v => JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options),
           v => string.IsNullOrEmpty(v) ? default : JsonSerializer.Deserialize<List<string>>(v, DefaultJsonSerializerOptions.Options));

        ValueComparer<List<string>> comparer = new ValueComparer<List<string>>(
            (l, r) => JsonSerializer.Serialize(l, DefaultJsonSerializerOptions.Options) == JsonSerializer.Serialize(r, DefaultJsonSerializerOptions.Options),
            v => v == null ? 0 : JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options).GetHashCode(),
            v => JsonSerializer.Deserialize<List<string>>(JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options), DefaultJsonSerializerOptions.Options));

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueConverter(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);
        return propertyBuilder;
    }
}
