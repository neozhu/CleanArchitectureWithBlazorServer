using System.Text.Json;
using System.Text.Json.Serialization;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;

namespace CleanArchitecture.Blazor.$safeprojectname$.Services.Serialization;
internal sealed class SystemTextJsonSerializer : ISerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public string Serialize<T>(T value) where T : class => JsonSerializer.Serialize(value, Options);

    public T? Deserialize<T>(string value) where T : class => JsonSerializer.Deserialize<T>(value, Options);

    public byte[] SerializeBytes<T>(T value) where T : class => JsonSerializer.SerializeToUtf8Bytes(value, Options);

    public T? DeserializeBytes<T>(byte[] value) where T : class => JsonSerializer.Deserialize<T>(value, Options);
}
