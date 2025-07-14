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
        var count = await specificationEvaluator.GetQuery(query.AsNoTracking(), spec).CountAsync();
        var data = await specificationEvaluator.GetQuery(query.AsNoTracking(), spec).Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<TResult>(configuration)
            .ToListAsync(cancellationToken);
        return new PaginatedData<TResult>(data, count, pageNumber, pageSize);
    }

    /// <summary>
    /// Projects the query to a paginated data asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of entities in the query</typeparam>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="query">The original ordered query</param>
    /// <param name="spec">The specification to apply to the query</param>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The size of the page</param>
    /// <param name="mapperFunc">The function to map entities to result</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the paginated data</returns>
    public static async Task<PaginatedData<TResult>> ProjectToPaginatedDataAsync<T, TResult>(
        this IOrderedQueryable<T> query, ISpecification<T> spec, int pageNumber, int pageSize,
        Func<T, TResult> mapperFunc, CancellationToken cancellationToken = default) where T : class, IEntity
    {
        var specificationEvaluator = SpecificationEvaluator.Default;
        var queryWithSpec = specificationEvaluator.GetQuery(query.AsNoTracking(), spec);

        var count = await queryWithSpec.CountAsync(cancellationToken);
        var data = await queryWithSpec
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = data.Select(x => mapperFunc(x)).ToList();

        return new PaginatedData<TResult>(items, count, pageNumber, pageSize);
    }

    /// <summary>
    /// Filters the queryable data based on the specified keyword.
    /// </summary>
    /// <typeparam name="T">The type of entities in the query</typeparam>
    /// <param name="source">The original queryable data</param>
    /// <param name="keyword">The keyword to search for</param>
    /// <returns>The filtered queryable data</returns>
    public static IQueryable<T> WhereContainsKeyword<T>(this IQueryable<T> source, string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
            return source;

        var parameter = Expression.Parameter(typeof(T), "x");
        var properties = typeof(T).GetProperties()
            .Where(p => p.PropertyType == typeof(string));

        Expression? predicate = null;

        foreach (var property in properties)
        {
            var propertyAccess = Expression.Property(parameter, property);
            var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
            var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
            var containsCall = Expression.Call(propertyAccess, containsMethod!, Expression.Constant(keyword));

            var condition = Expression.AndAlso(nullCheck, containsCall);

            predicate = predicate == null ? condition : Expression.OrElse(predicate, condition);
        }

        if (predicate == null)
            return source;

        var lambda = Expression.Lambda<Func<T, bool>>(predicate, parameter);
        return source.Where(lambda);
    }



    #region OrderBy
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderByProperty)
    {
        var parts = orderByProperty.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string property = parts[0];
        string direction = parts.Length > 1 && parts[1].Equals("Descending", StringComparison.OrdinalIgnoreCase)
            ? "OrderByDescending"
            : "OrderBy";

        return ApplyOrder(source, property, direction);
    }

    public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string orderByProperty)
    {
        return ApplyOrder(source, orderByProperty, "OrderByDescending");
    }

    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string orderByProperty)
    {
        return ApplyOrder(source, orderByProperty, "ThenBy");
    }

    public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string orderByProperty)
    {
        return ApplyOrder(source, orderByProperty, "ThenByDescending");
    }

    private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
    {
        var type = typeof(T);
        var propertyInfo = type.GetProperty(property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (propertyInfo == null)
        {
            throw new ArgumentException($"Property '{property}' does not exist on type '{type.Name}'.");
        }

        var parameter = Expression.Parameter(type, "x");
        var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);

        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { type, propertyInfo.PropertyType },
            source.Expression,
            Expression.Quote(orderByExpression));

        return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(resultExpression);
    }

    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderBy, string sortDirection)
    {
        return sortDirection.Equals("Descending", StringComparison.OrdinalIgnoreCase)
            ? source.OrderByDescending(orderBy)
            : source.OrderBy(orderBy);
    }
    #endregion


}
