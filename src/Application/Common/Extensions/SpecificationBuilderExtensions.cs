namespace CleanArchitecture.Blazor.Application.Common.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ISpecificationBuilder{T}"/> to simplify query building.
/// </summary>
public static class SpecificationBuilderExtensions
{
    /// <summary>
    /// Adds a 'Contains' filter to the query for the specified keyword.
    /// Searches the specified string properties, or all string properties if none are specified.
    /// </summary>
    /// <typeparam name="T">The type of the entity being queried.</typeparam>
    /// <param name="builder">The specification builder to which the filter is applied.</param>
    /// <param name="keyword">The keyword to search for within the string properties.</param>
    /// <param name="condition">Determines whether to apply the filter. Defaults to <c>true</c>.</param>
    /// <param name="properties">
    /// An array of expressions specifying the string properties to search.
    /// If not provided, all string properties of the entity type <typeparamref name="T"/> are searched.
    /// </param>
    /// <returns>The modified <see cref="ISpecificationBuilder{T}"/> with the applied filter.</returns>
    public static ISpecificationBuilder<T> Where<T>(
        this ISpecificationBuilder<T> builder,
        string keyword,
        bool condition = true,
        params Expression<Func<T, string?>>[] properties)
    {
        // If the condition is false or the keyword is null or empty, return the original builder without modifications
        if (!condition || string.IsNullOrEmpty(keyword))
            return builder;

        // If no properties are specified, search all string properties by default
        if (properties == null || properties.Length == 0)
        {
            properties = typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .Select(p => CreatePropertyExpression<T>(p))
                .ToArray();
        }

        Expression? predicate = null;
        var parameter = Expression.Parameter(typeof(T), "x");

        // Iterate over each specified property to build the search predicate
        foreach (var propertyExpression in properties)
        {
            if (propertyExpression.Body is not MemberExpression memberExpression)
                continue; // Skip if the expression is not a member expression

            var propertyAccess = Expression.Property(parameter, (memberExpression.Member as PropertyInfo)!);

            // Create a null check: property != null
            var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));

            // Get the 'Contains' method info
            var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });

            // Create the 'Contains' method call expression: property.Contains(keyword)
            var containsCall = Expression.Call(propertyAccess, containsMethod!, Expression.Constant(keyword));

            // Combine null check and 'Contains' call: property != null && property.Contains(keyword)
            var conditionExpression = Expression.AndAlso(nullCheck, containsCall);

            // Combine with existing predicates using logical OR
            predicate = predicate == null ? conditionExpression : Expression.OrElse(predicate, conditionExpression);
        }

        // If a predicate was built, apply it to the specification builder
        if (predicate != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(predicate, parameter);
            builder.Where(lambda);
        }

        return builder;
    }

    /// <summary>
    /// Creates a property access expression for the specified property.
    /// </summary>
    /// <typeparam name="T">The type of the entity containing the property.</typeparam>
    /// <param name="property">The property information object.</param>
    /// <returns>An expression representing access to the property.</returns>
    private static Expression<Func<T, string?>> CreatePropertyExpression<T>(PropertyInfo property)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.Property(parameter, property);
        return Expression.Lambda<Func<T, string?>>(propertyAccess, parameter);
    }
}

