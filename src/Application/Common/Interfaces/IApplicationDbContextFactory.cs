namespace CleanArchitecture.Blazor.Application.Common.Interfaces;
public interface IApplicationDbContextFactory
{
    ValueTask<IApplicationDbContext> CreateAsync(CancellationToken ct = default);
}
