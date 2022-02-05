// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Application.Common.Models;

public class PaginatedData<T>
{
    public int total { get; set; }
    public IEnumerable<T> rows { get; set; }
    public PaginatedData(IEnumerable<T> items, int total)
    {
        this.rows = items;
        this.total = total;
    }
    public static async Task<PaginatedData<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedData<T>(items, count);
    }
}
