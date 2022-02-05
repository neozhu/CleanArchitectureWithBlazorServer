// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Razor.Infrastructure.Services;

public class DictionaryService : IDictionaryService
{
    private readonly IApplicationDbContext _context;

    public DictionaryService(
        IApplicationDbContext context)

    {
        _context = context;
    }
    public async Task<IDictionary<string, string>> Fetch(string name)
    {
        var result = await _context.KeyValues.Where(x => x.Name == name)
            .ToDictionaryAsync(k => k.Value, v => v.Text);
        return result;
    }
}
