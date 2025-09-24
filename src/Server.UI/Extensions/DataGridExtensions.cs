using System.Linq.Expressions;
using System.Reflection;

namespace CleanArchitecture.Blazor.Server.UI.Extensions;

/// <summary>
/// Extension methods for applying sort operations to IQueryable data sources in data grids
/// </summary>
public static class DataGridExtensions
{
    // Cache reflection results to improve performance
    private static readonly Dictionary<string, MethodInfo> SortMethodCache = new();
    
    /// <summary>
    /// Applies sort definitions from GridState to the queryable source
    /// </summary>
    /// <typeparam name="T">The entity type being queried</typeparam>
    /// <typeparam name="TGridModel">The grid model type</typeparam>
    /// <param name="source">The queryable data source</param>
    /// <param name="state">The grid state containing sort definitions</param>
    /// <returns>An ordered queryable with applied sort operations</returns>
    public static IQueryable<T> ApplySortDefinitions<T, TGridModel>(this IQueryable<T> source, GridState<TGridModel> state)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(state);
        
        return source.ApplySortDefinitions(state.SortDefinitions);
    }

    /// <summary>
    /// Applies multiple sort definitions to the queryable source
    /// </summary>
    /// <typeparam name="T">The entity type being queried</typeparam>
    /// <typeparam name="TGridModel">The grid model type</typeparam>
    /// <param name="source">The queryable data source</param>
    /// <param name="sortDefinitions">Collection of sort definitions to apply</param>
    /// <returns>An ordered queryable with applied sort operations</returns>
    public static IQueryable<T> ApplySortDefinitions<T, TGridModel>(this IQueryable<T> source,
        ICollection<SortDefinition<TGridModel>> sortDefinitions)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(sortDefinitions);

        if (sortDefinitions.Count == 0)
            return source;

        IOrderedQueryable<T>? orderedQuery = null;

        foreach (var sortDefinition in sortDefinitions)
        {
            if (string.IsNullOrWhiteSpace(sortDefinition.SortBy))
                continue;

            try
            {
                var sortExpression = CreateSortExpression<T>(sortDefinition.SortBy);
                
                orderedQuery = orderedQuery == null
                    ? ApplyInitialSort(source, sortExpression, sortDefinition.Descending)
                    : ApplySubsequentSort(orderedQuery, sortExpression, sortDefinition.Descending);
            }
            catch (ArgumentException)
            {
                // Skip invalid property names rather than throwing
                continue;
            }
        }

        return orderedQuery ?? source;
    }

    /// <summary>
    /// Creates a sort expression for the specified property
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="propertyName">The name of the property to sort by</param>
    /// <returns>A lambda expression for sorting</returns>
    private static LambdaExpression CreateSortExpression<T>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = GetPropertyExpression(parameter, propertyName);
        return Expression.Lambda(property, parameter);
    }

    /// <summary>
    /// Gets the property expression, supporting nested properties (e.g., "Address.City")
    /// </summary>
    /// <param name="parameter">The parameter expression</param>
    /// <param name="propertyPath">The property path (supports dot notation)</param>
    /// <returns>The property expression</returns>
    private static Expression GetPropertyExpression(Expression parameter, string propertyPath)
    {
        var properties = propertyPath.Split('.');
        var expression = parameter;

        foreach (var propertyName in properties)
        {
            var propertyInfo = expression.Type.GetProperty(propertyName, 
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            
            if (propertyInfo == null)
                throw new ArgumentException($"Property '{propertyName}' not found on type '{expression.Type.Name}'");
                
            expression = Expression.Property(expression, propertyInfo);
        }

        return expression;
    }

    /// <summary>
    /// Applies the initial sort operation (OrderBy/OrderByDescending)
    /// </summary>
    private static IOrderedQueryable<T> ApplyInitialSort<T>(IQueryable<T> source, LambdaExpression sortExpression, bool descending)
    {
        var methodName = descending ? "OrderByDescending" : "OrderBy";
        var sortMethod = GetSortMethod(methodName, typeof(T), sortExpression.ReturnType);
        
        return (IOrderedQueryable<T>)sortMethod.Invoke(null, new object[] { source, sortExpression })!;
    }

    /// <summary>
    /// Applies subsequent sort operations (ThenBy/ThenByDescending)
    /// </summary>
    private static IOrderedQueryable<T> ApplySubsequentSort<T>(IOrderedQueryable<T> source, LambdaExpression sortExpression, bool descending)
    {
        var methodName = descending ? "ThenByDescending" : "ThenBy";
        var sortMethod = GetSortMethod(methodName, typeof(T), sortExpression.ReturnType);
        
        return (IOrderedQueryable<T>)sortMethod.Invoke(null, new object[] { source, sortExpression })!;
    }

    /// <summary>
    /// Gets the appropriate sort method using caching for performance
    /// </summary>
    private static MethodInfo GetSortMethod(string methodName, Type entityType, Type propertyType)
    {
        var cacheKey = $"{methodName}_{entityType.Name}_{propertyType.Name}";
        
        if (SortMethodCache.TryGetValue(cacheKey, out var cachedMethod))
            return cachedMethod;

        var method = typeof(Queryable)
            .GetMethods()
            .Where(m => m.Name == methodName && m.IsGenericMethodDefinition)
            .Single(m => m.GetParameters().Length == 2)
            .MakeGenericMethod(entityType, propertyType);

        SortMethodCache[cacheKey] = method;
        return method;
    }
}
