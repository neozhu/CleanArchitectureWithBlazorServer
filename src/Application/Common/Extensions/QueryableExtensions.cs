// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Application.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> spec) where T : class, IEntity
    {
        var queryableResultWithIncludes = spec.Includes
           .Aggregate(query,
               (current, include) => include(current));
        var secondaryResult = spec.IncludeStrings
            .Aggregate(queryableResultWithIncludes,
                (current, include) => current.Include(include));
        return secondaryResult.Where(spec.Criteria);
    }
}
