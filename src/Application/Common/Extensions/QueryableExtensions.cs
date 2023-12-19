// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Ardalis.Specification.EntityFrameworkCore;
using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Application.Common.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    ///     Extension method to provided query by specification.
    /// </summary>
    /// <remarks>
    ///     This method uses the SpecificationEvaluator to evaluate and modify the provided query based on the given
    ///     specification.
    /// </remarks>
    /// <typeparam name="T">Type of the entities in the query</typeparam>
    /// <param name="query">The original query to which the specification should be applied</param>
    /// <param name="spec">The specification to apply to the query</param>
    /// <param name="evaluateCriteriaOnly">
    ///     Optional flag to determine if only the criteria should be evaluated or other
    ///     configurations as well
    /// </param>
    /// <returns>The modified query with the specification applied</returns>
    public static IQueryable<T> ApplySpecification<T>(this IQueryable<T> query, ISpecification<T> spec,
        bool evaluateCriteriaOnly = false) where T : class, IEntity
    {
        return SpecificationEvaluator.Default.GetQuery(query, spec, evaluateCriteriaOnly);
    }

    /// <summary>
    ///     Extension method to provided ordered queryable data to a paginated result set.
    /// </summary>
    /// <remarks>
    ///     This method will apply the given specification to the query, paginate the results, and project them to the desired
    ///     result type.
    /// </remarks>
    /// <typeparam name="T">Source type of the entities in the query</typeparam>
    /// <typeparam name="TResult">Destination type to which the entities should be projected</typeparam>
    /// <param name="query">The original ordered query to project and paginate</param>
    /// <param name="spec">The specification to apply to the query before projection and pagination</param>
    /// <param name="pageNumber">The desired page number of the paginated results</param>
    /// <param name="pageSize">The number of items per page in the paginated results</param>
    /// <param name="configuration">Configuration for the projection</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation</param>
    /// <returns>The paginated and projected data</returns>
    public static async Task<PaginatedData<TResult>> ProjectToPaginatedDataAsync<T, TResult>(
        this IOrderedQueryable<T> query, ISpecification<T> spec, int pageNumber, int pageSize,
        IConfigurationProvider configuration, CancellationToken cancellationToken = default) where T : class, IEntity
    {
        var specificationEvaluator = SpecificationEvaluator.Default;
        var count = await specificationEvaluator.GetQuery(query, spec).CountAsync();
        var data = await specificationEvaluator.GetQuery(query.AsNoTracking(), spec).Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<TResult>(configuration)
            .ToListAsync(cancellationToken);
        return new PaginatedData<TResult>(data, count, pageNumber, pageSize);
    }
}