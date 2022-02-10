// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Models;

public class PaginatedData<T>
{
    public int TotalItems { get; set; }
    public IEnumerable<T> Items { get; set; }
    public PaginatedData(IEnumerable<T> items, int total)
    {
        this.Items = items;
        this.TotalItems = total;
    }
    public static async Task<PaginatedData<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedData<T>(items, count);
    }
}
