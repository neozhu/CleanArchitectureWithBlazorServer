namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;

public interface ISerializer
{
    string Serialize<T>(T value) where T : class;
    T? Deserialize<T>(string value) where T : class;
    byte[] SerializeBytes<T>(T value) where T : class;
    T? DeserializeBytes<T>(byte[] value) where T : class;
}