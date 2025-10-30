using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface IDataSourceService<T>
{
    IReadOnlyList<T> DataSource { get; }
    Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>>? predicate, int? limit=null, CancellationToken cancellationToken = default);
    event Func<Task>? OnChange;
    Task InitializeAsync();
    Task RefreshAsync();
}
