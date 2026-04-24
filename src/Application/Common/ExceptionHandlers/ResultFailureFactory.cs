using System.Collections.Concurrent;

namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

internal static class ResultFailureFactory
{
    private static readonly ConcurrentDictionary<Type, Func<string[], object>> FailureFactoryCache = new();

    public static TResponse Create<TResponse>(params string[] errors)
        where TResponse : IResult
    {
        var factory = FailureFactoryCache.GetOrAdd(typeof(TResponse), CreateFailureFactory);
        return (TResponse)factory(errors);
    }

    private static Func<string[], object> CreateFailureFactory(Type responseType)
    {
        var errorsParam = Expression.Parameter(typeof(string[]), "errors");

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var genericResultType = typeof(Result<>).MakeGenericType(valueType);
            var failureMethod = genericResultType.GetMethod(
                "Failure",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(string[])],
                modifiers: null)
                ?? throw new InvalidOperationException($"Could not find the 'Failure' method on Result<{valueType.Name}>.");

            var call = Expression.Call(null, failureMethod, errorsParam);
            var cast = Expression.Convert(call, typeof(object));
            return Expression.Lambda<Func<string[], object>>(cast, errorsParam).Compile();
        }

        var resultFailureMethod = typeof(Result).GetMethod(
            "Failure",
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            types: [typeof(string[])],
            modifiers: null)
            ?? throw new InvalidOperationException("Could not find the 'Failure' method on Result.");

        var resultCall = Expression.Call(null, resultFailureMethod, errorsParam);
        var resultCast = Expression.Convert(resultCall, typeof(object));
        return Expression.Lambda<Func<string[], object>>(resultCast, errorsParam).Compile();
    }
}
