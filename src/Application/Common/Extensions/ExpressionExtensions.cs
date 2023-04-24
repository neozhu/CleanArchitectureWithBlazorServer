// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace CleanArchitecture.Blazor.Application.Common.Extensions;
#nullable disable
public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> FromFilter<T>(string filters)
    {
        Expression<Func<T, bool>> any = x => true;
        if (!string.IsNullOrEmpty(filters))
        {
            var opts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            opts.Converters.Add(new AutoNumberToStringConverter());
            var filterRules = JsonSerializer.Deserialize<FilterRule[]>(filters, opts);
            if (filterRules is not null)
            {
                foreach (var filter in filterRules)
                {
                    if (Enum.TryParse(filter.Op, out OperationExpression op) && !string.IsNullOrEmpty(filter.Value))
                    {
                        var expression = GetCriteriaWhere<T>(filter.Field, op, filter.Value);
                        any = any.And(expression);
                    }
                }
            }
        }

        return any;
    }
    #region -- Public methods --
    private static Expression<Func<T, bool>> GetCriteriaWhere<T>(Expression<Func<T, object>> e, OperationExpression selectedOperator, object fieldValue)
    {
        var name = GetOperand<T>(e);
        return GetCriteriaWhere<T>(name, selectedOperator, fieldValue);
    }

    private static Expression<Func<T, bool>> GetCriteriaWhere<T, T2>(Expression<Func<T, object>> e, OperationExpression selectedOperator, object fieldValue)
    {
        var name = GetOperand<T>(e);
        return GetCriteriaWhere<T, T2>(name, selectedOperator, fieldValue);
    }

    private static Expression<Func<T, bool>> GetCriteriaWhere<T>(string fieldName, OperationExpression selectedOperator, object fieldValue)
    {
        var props = TypeDescriptor.GetProperties(typeof(T));
        var prop = GetProperty(props, fieldName, true);
        var parameter = Expression.Parameter(typeof(T));
        var expressionParameter = GetMemberExpression<T>(parameter, fieldName);
        if (prop != null && fieldValue != null)
        {
            BinaryExpression body = null;
            if (prop.PropertyType.IsEnum)
            {
                if (Enum.IsDefined(prop.PropertyType, fieldValue))
                {
                    object value = Enum.Parse(prop.PropertyType, fieldValue.ToString(), true);
                    body = Expression.Equal(expressionParameter, Expression.Constant(value));
                    return Expression.Lambda<Func<T, bool>>(body, parameter);
                }
                else
                {
                    return x => false;
                }
            }
            switch (selectedOperator)
            {
                case OperationExpression.Equal:
                    body = Expression.Equal(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue.ToString() == "null" ? null : fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                    return Expression.Lambda<Func<T, bool>>(body, parameter);
                case OperationExpression.NotEqual:
                    body = Expression.NotEqual(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue.ToString() == "null" ? null : fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                    return Expression.Lambda<Func<T, bool>>(body, parameter);
                case OperationExpression.Less:
                    body = Expression.LessThan(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                    return Expression.Lambda<Func<T, bool>>(body, parameter);
                case OperationExpression.LessOrEqual:
                    body = Expression.LessThanOrEqual(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                    return Expression.Lambda<Func<T, bool>>(body, parameter);
                case OperationExpression.Greater:
                    body = Expression.GreaterThan(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                    return Expression.Lambda<Func<T, bool>>(body, parameter);
                case OperationExpression.GreaterOrEqual:
                    body = Expression.GreaterThanOrEqual(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                    return Expression.Lambda<Func<T, bool>>(body, parameter);
                case OperationExpression.Contains:
                    var contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var bodyLike = Expression.Call(expressionParameter, contains, Expression.Constant(Convert.ChangeType(fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                    return Expression.Lambda<Func<T, bool>>(bodyLike, parameter);
                case OperationExpression.EndsWith:
                    var endsWith = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                    var bodyEndsWith = Expression.Call(expressionParameter, endsWith, Expression.Constant(Convert.ChangeType(fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                    return Expression.Lambda<Func<T, bool>>(bodyEndsWith, parameter);
                case OperationExpression.BeginsWith:
                    var startsWith = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                    var bodyStartsWith = Expression.Call(expressionParameter, startsWith, Expression.Constant(Convert.ChangeType(fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                    return Expression.Lambda<Func<T, bool>>(bodyStartsWith, parameter);
                case OperationExpression.Includes:
                    return Includes<T>(fieldValue, parameter, expressionParameter, prop.PropertyType);
                case OperationExpression.Between:
                    return Between<T>(fieldValue, parameter, expressionParameter, prop.PropertyType);
                default:
                    throw new ArgumentException("OperationExpression");
            }
        }

        return x => false;
    }

    private static Expression<Func<T, bool>> GetCriteriaWhere<T, T2>(string fieldName, OperationExpression selectedOperator, object fieldValue)
    {


        var props = TypeDescriptor.GetProperties(typeof(T));
        var prop = GetProperty(props, fieldName, true);

        var parameter = Expression.Parameter(typeof(T));
        var expressionParameter = GetMemberExpression<T>(parameter, fieldName);

        if (prop != null && fieldValue != null)
        {
            return selectedOperator switch
            {
                OperationExpression.Any => Any<T, T2>(fieldValue, parameter, expressionParameter),
                _ => throw new Exception("Not implement Operation")
            };
        }

        Expression<Func<T, bool>> filter = x => true;
        return filter;
    }
    


    #endregion
    #region -- Private methods --

    private static string GetOperand<T>(Expression<Func<T, object>> exp)
    {
        if (!(exp.Body is MemberExpression body))
        {
            var uBody = (UnaryExpression)exp.Body;
            body = uBody.Operand as MemberExpression;
        }

        var operand = body.ToString();

        return operand.Substring(2);

    }

    private static MemberExpression GetMemberExpression<T>(ParameterExpression parameter, string propName)
    {
        if (string.IsNullOrEmpty(propName))
        {
            return null;
        }

        var propertiesName = propName.Split('.');
        if (propertiesName.Length == 2)
        {
            return Expression.Property(Expression.Property(parameter, propertiesName[0]), propertiesName[1]);
        }

        return Expression.Property(parameter, propName);
    }

    private static Expression<Func<T, bool>> Includes<T>(object fieldValue, ParameterExpression parameterExpression, MemberExpression memberExpression, Type type)
    {
        var safeType = Nullable.GetUnderlyingType(type) ?? type;

        switch (safeType.Name.ToLower())
        {
            case "string":
                var strList = fieldValue.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                if (strList == null || strList.Count == 0)
                {
                    return x => true;
                }
                var strMethod = typeof(List<string>).GetMethod("Contains", new Type[] { typeof(string) });
                var strCallExp = Expression.Call(Expression.Constant(strList.ToList()), strMethod, memberExpression);
                return Expression.Lambda<Func<T, bool>>(strCallExp, parameterExpression);
            case "int32":
                var intList = fieldValue.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();
                if (intList == null || intList.Count == 0)
                {
                    return x => true;
                }
                var intMethod = typeof(List<int>).GetMethod("Contains", new Type[] { typeof(int) });
                var intCallExp = Expression.Call(Expression.Constant(intList.ToList()), intMethod, memberExpression);
                return Expression.Lambda<Func<T, bool>>(intCallExp, parameterExpression);
            case "float":
            case "decimal":
                var floatList = fieldValue.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Decimal.Parse).ToList();
                if (floatList == null || floatList.Count == 0)
                {
                    return x => true;
                }
                var floatMethod = typeof(List<decimal>).GetMethod("Contains", new Type[] { typeof(decimal) });
                var floatCallExp = Expression.Call(Expression.Constant(floatList.ToList()), floatMethod, memberExpression);
                return Expression.Lambda<Func<T, bool>>(floatCallExp, parameterExpression);
            default:
                return x => true;
        }

    }
    private static Expression<Func<T, bool>> Between<T>(object fieldValue, ParameterExpression parameterExpression, MemberExpression memberExpression, Type type)
    {

        var safeType = Nullable.GetUnderlyingType(type) ?? type;
        switch (safeType.Name.ToLower())
        {
            case "datetime":
                var dateArray = ((string)fieldValue).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                var start = Convert.ToDateTime(dateArray[0] + " 00:00:00", CultureInfo.CurrentCulture);
                var end = Convert.ToDateTime(dateArray[1] + " 23:59:59", CultureInfo.CurrentCulture);
                var greater = Expression.GreaterThanOrEqual(memberExpression, Expression.Constant(start, type));
                var less = Expression.LessThanOrEqual(memberExpression, Expression.Constant(end, type));
                return Expression.Lambda<Func<T, bool>>(greater, parameterExpression)
                  .And(Expression.Lambda<Func<T, bool>>(less, parameterExpression));
            case "int":
            case "int32":
                var intArray = ((string)fieldValue).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                var min = Convert.ToInt32(intArray[0], CultureInfo.CurrentCulture);
                var max = Convert.ToInt32(intArray[1], CultureInfo.CurrentCulture);
                var maxThan = Expression.GreaterThanOrEqual(memberExpression, Expression.Constant(min, type));
                var lessThan = Expression.LessThanOrEqual(memberExpression, Expression.Constant(max, type));
                return Expression.Lambda<Func<T, bool>>(maxThan, parameterExpression)
                  .And(Expression.Lambda<Func<T, bool>>(lessThan, parameterExpression));
            case "decimal":
                var decArray = ((string)fieldValue).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                var dMin = Convert.ToDecimal(decArray[0], CultureInfo.CurrentCulture);
                var dMax = Convert.ToDecimal(decArray[1], CultureInfo.CurrentCulture);
                var dMaxThan = Expression.GreaterThanOrEqual(memberExpression, Expression.Constant(dMin, type));
                var dLessThan = Expression.LessThanOrEqual(memberExpression, Expression.Constant(dMax, type));
                return Expression.Lambda<Func<T, bool>>(dMaxThan, parameterExpression)
                  .And(Expression.Lambda<Func<T, bool>>(dLessThan, parameterExpression));
            case "float":
                var fArray = ((string)fieldValue).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                var fMin = Convert.ToDecimal(fArray[0], CultureInfo.CurrentCulture);
                var fMax = Convert.ToDecimal(fArray[1], CultureInfo.CurrentCulture);
                var fMaxThan = Expression.GreaterThanOrEqual(memberExpression, Expression.Constant(fMin, type));
                var fLessThan = Expression.LessThanOrEqual(memberExpression, Expression.Constant(fMax, type));
                return Expression.Lambda<Func<T, bool>>(fMaxThan, parameterExpression)
                  .And(Expression.Lambda<Func<T, bool>>(fLessThan, parameterExpression));
            case "string":
                var strArray = ((string)fieldValue).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                var strStart = strArray[0];
                var strEnd = strArray[1];
                var strMethod = typeof(string).GetMethod("CompareTo", new[] { typeof(string) });
                var callCompareToStart = Expression.Call(memberExpression, strMethod, Expression.Constant(strStart, type));
                var callCompareToEnd = Expression.Call(memberExpression, strMethod, Expression.Constant(strEnd, type));
                var strGreater = Expression.GreaterThanOrEqual(callCompareToStart, Expression.Constant(0));
                var strLess = Expression.LessThanOrEqual(callCompareToEnd, Expression.Constant(0));
                return Expression.Lambda<Func<T, bool>>(strGreater, parameterExpression)
                  .And(Expression.Lambda<Func<T, bool>>(strLess, parameterExpression));
            default:
                return x => true;
        }

    }



    private static Expression<Func<T, bool>> Any<T, T2>(object fieldValue, ParameterExpression parameterExpression, MemberExpression memberExpression)
    {
        var lambda = (Expression<Func<T2, bool>>)fieldValue;
        var anyMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(m => m.Name == "Any" && m.GetParameters().Length == 2).MakeGenericMethod(typeof(T2));

        var body = Expression.Call(anyMethod, memberExpression, lambda);

        return Expression.Lambda<Func<T, bool>>(body, parameterExpression);
    }

    private static PropertyDescriptor GetProperty(PropertyDescriptorCollection props, string fieldName, bool ignoreCase)
    {
        if (!fieldName.Contains('.'))
        {
            return props.Find(fieldName, ignoreCase);
        }

        var fieldNameProperty = fieldName.Split('.');
        return props.Find(fieldNameProperty[0], ignoreCase).GetChildProperties().Find(fieldNameProperty[1], ignoreCase);

    }
    #endregion

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        ParameterExpression p = left.Parameters.First();
        SubstExpressionVisitor visitor = new SubstExpressionVisitor
        {
            Subst = { [right.Parameters.First()] = p }
        };

        Expression body = Expression.AndAlso(left.Body, visitor.Visit(right.Body));
        return Expression.Lambda<Func<T, bool>>(body, p);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {

        ParameterExpression p = left.Parameters.First();
        SubstExpressionVisitor visitor = new SubstExpressionVisitor
        {
            Subst = { [right.Parameters.First()] = p }
        };

        Expression body = Expression.OrElse(left.Body, visitor.Visit(right.Body));
        return Expression.Lambda<Func<T, bool>>(body, p);
    }
}

internal class SubstExpressionVisitor : ExpressionVisitor
{
    public Dictionary<Expression, Expression> Subst = new();

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (Subst.TryGetValue(node, out var newValue))
        {
            return newValue;
        }
        return node;
    }
}

internal class SwapVisitor : ExpressionVisitor
{
    private readonly Expression from, to;
    public SwapVisitor(Expression from, Expression to)
    {
        this.from = from;
        this.to = to;
    }
    public override Expression Visit(Expression node) => node == from ? to : base.Visit(node);
}
internal sealed class AutoNumberToStringConverter : JsonConverter<object>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(string) == typeToConvert;
    }
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.TryGetInt64(out long l) ?
                l.ToString() :
                reader.GetDouble().ToString();
        }
        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }
        using (JsonDocument document = JsonDocument.ParseValue(ref reader))
        {
            return document.RootElement.Clone().ToString();
        }
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

internal enum OperationExpression
{
    Equal,
    NotEqual,
    Less,
    LessOrEqual,
    Greater,
    GreaterOrEqual,
    Contains,
    BeginsWith,
    EndsWith,
    Includes,
    Between,
    Any
}
