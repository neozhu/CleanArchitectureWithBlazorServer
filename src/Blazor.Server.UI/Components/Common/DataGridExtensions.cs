using System.Linq.Expressions;

namespace Blazor.Server.UI;
public static class DataGridExtensions
{
    public static IQueryable<T> EFOrderBySortDefinitions<T,T1>(this IQueryable<T> source, GridState<T1> state)
        => EFOrderBySortDefinitions(source, state.SortDefinitions);

    public static IQueryable<T> EFOrderBySortDefinitions<T,T1>(this IQueryable<T> source, ICollection<SortDefinition<T1>> sortDefinitions)
    {
        // avoid multiple enumeration
        var sourceQuery = source;

        if (sortDefinitions.Count == 0)
            return sourceQuery;

        IOrderedQueryable<T>? orderedQuery = null;

        foreach (var sortDefinition in sortDefinitions)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var orderByProperty = Expression.Property(parameter, sortDefinition.SortBy);
            var sortlambda= Expression.Lambda(orderByProperty, parameter);
            if (orderedQuery is null)
            {
                var sortmethod = typeof(Queryable).GetMethods()
                .Where(m => m.Name == (sortDefinition.Descending? "OrderByDescending" : "OrderBy") && m.IsGenericMethodDefinition)
                .Where(m => m.GetParameters().ToList().Count == 2) // ensure selecting the right overload
                .Single();
                var genericMethod = sortmethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
                orderedQuery = (IOrderedQueryable<T>?)genericMethod.Invoke(genericMethod, new object[] { source, sortlambda });
            }
            else
            {
                var sortmethod = typeof(Queryable).GetMethods()
                .Where(m => m.Name == (sortDefinition.Descending ? "ThenByDescending" : "ThenBy") && m.IsGenericMethodDefinition)
                .Where(m => m.GetParameters().ToList().Count == 2) // ensure selecting the right overload
                .Single();
                var genericMethod = sortmethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
                orderedQuery = (IOrderedQueryable<T>?)genericMethod.Invoke(genericMethod, new object[] { source, sortlambda });
            }
        }
        return orderedQuery ?? sourceQuery;
    }


}

