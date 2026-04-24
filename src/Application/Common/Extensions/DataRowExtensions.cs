// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Extensions;

public static class DataRowExtensions
{
    public static T? GetValue<T>(this DataRow row, string columnName, T? defaultValue = default)
    {
        if (!row.Table.Columns.Contains(columnName) || row.IsNull(columnName))
        {
            return defaultValue;
        }

        object value = row[columnName];
        if (value is string str && string.IsNullOrWhiteSpace(str))
        {
            return defaultValue;
        }

        Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        try
        {
            return (T)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }
        catch
        {
            return defaultValue;
        }
    }
}
