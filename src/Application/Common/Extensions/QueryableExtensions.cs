// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Ardalis.Specification.EntityFrameworkCore;
using Ardalis.Specification;
using CleanArchitecture.Blazor.Domain.Common;
using DocumentFormat.OpenXml.Bibliography;

namespace CleanArchitecture.Blazor.Application.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> spec, bool evaluateCriteriaOnly=false) where T : class, IEntity
    {
        return new SpecificationEvaluator().GetQuery(query, spec, evaluateCriteriaOnly);
    }
}
