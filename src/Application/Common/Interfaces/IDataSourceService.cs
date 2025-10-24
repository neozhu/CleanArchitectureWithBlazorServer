using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface IDataSourceService<T>
{
    IReadOnlyList<T> DataSource { get; }
    event Func<Task>? OnChange;
    Task InitializeAsync();
    Task RefreshAsync();
}
