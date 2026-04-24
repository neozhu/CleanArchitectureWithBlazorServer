namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;

public interface IAppCache
{
    Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default);

    void Remove(string key);

    Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default);

    Task RemoveByTagsAsync(IEnumerable<string>? tags, CancellationToken cancellationToken = default);
}
