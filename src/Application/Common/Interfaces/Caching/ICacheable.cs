// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;

public interface ICacheable
{
    string CacheKey { get=>String.Empty; }
    MemoryCacheEntryOptions? Options { get; }
}
