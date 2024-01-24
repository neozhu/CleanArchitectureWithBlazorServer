// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Extensions;

public static class DataRowExtensions
{
    public static T? FieldOrDefault<T>(this DataRow row, string columnName)
    {
        return row.IsNull(columnName) || string.IsNullOrEmpty(row[columnName].ToString())
            ? default
            : row.Field<T>(columnName);
    }

    public static decimal? FieldDecimalOrNull(this DataRow row, string columnName)
    {
        return row.IsNull(columnName) || string.IsNullOrEmpty(row[columnName].ToString())
            ? default(decimal?)
            : Convert.ToDecimal(row[columnName]);
    }

    public static decimal FieldDecimalOrDefault(this DataRow row, string columnName)
    {
        return row.IsNull(columnName) || string.IsNullOrEmpty(row[columnName].ToString())
            ? default
            : Convert.ToDecimal(row[columnName]);
    }

    public static int? FieldIntOrNull(this DataRow row, string columnName)
    {
        return row.IsNull(columnName) || string.IsNullOrEmpty(row[columnName].ToString())
            ? default(int?)
            : Convert.ToInt32(row[columnName]);
    }

    public static int FieldIntOrDefault(this DataRow row, string columnName)
    {
        return row.IsNull(columnName) || string.IsNullOrEmpty(row[columnName].ToString())
            ? default
            : Convert.ToInt32(row[columnName]);
    }

    public static DateTime? FieldDateTimeOrNull(this DataRow row, string columnName)
    {
        return row.IsNull(columnName) || string.IsNullOrEmpty(row[columnName].ToString())
            ? default(DateTime?)
            : Convert.ToDateTime(row[columnName], CultureInfo.InvariantCulture);
    }

    public static DateTime FieldDateTimeOrDefaultNow(this DataRow row, string columnName)
    {
        return row.IsNull(columnName) || string.IsNullOrEmpty(row[columnName].ToString())
            ? DateTime.Now
            : Convert.ToDateTime(row[columnName], CultureInfo.InvariantCulture);
    }
}