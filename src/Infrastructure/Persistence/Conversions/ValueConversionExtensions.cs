using System.Text.Json;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;
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
            v => JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options),
            v => string.IsNullOrEmpty(v)
                ? default
                : JsonSerializer.Deserialize<T>(v, DefaultJsonSerializerOptions.Options));

        var comparer = new ValueComparer<T>(
            (l, r) => JsonSerializer.Serialize(l, DefaultJsonSerializerOptions.Options) ==
                      JsonSerializer.Serialize(r, DefaultJsonSerializerOptions.Options),
            v => v == null ? 0 : JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options).GetHashCode(),
            v => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options),
                DefaultJsonSerializerOptions.Options));

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueConverter(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);
        return propertyBuilder;
    }

    public static PropertyBuilder<List<string>> HasStringListConversion(
        this PropertyBuilder<List<string>> propertyBuilder)
    {
        var converter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options),
            v => string.IsNullOrEmpty(v)
                ? default
                : JsonSerializer.Deserialize<List<string>>(v, DefaultJsonSerializerOptions.Options));

        var comparer = new ValueComparer<List<string>>(
            (l, r) => JsonSerializer.Serialize(l, DefaultJsonSerializerOptions.Options) ==
                      JsonSerializer.Serialize(r, DefaultJsonSerializerOptions.Options),
            v => v == null ? 0 : JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options).GetHashCode(),
            v => JsonSerializer.Deserialize<List<string>>(
                JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options),
                DefaultJsonSerializerOptions.Options));

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueConverter(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);
        return propertyBuilder;
    }
}