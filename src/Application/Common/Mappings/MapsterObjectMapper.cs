using Mapster;

namespace CleanArchitecture.Blazor.Application.Common.Mappings;

public class MapsterObjectMapper : IObjectMapper
{
    private readonly TypeAdapterConfig _config;

    public MapsterObjectMapper(TypeAdapterConfig config)
    {
        _config = config;
    }

    public TDestination Map<TDestination>(object source)
    {
        return source.Adapt<TDestination>(_config);
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        return source.Adapt(destination, _config);
    }
}
