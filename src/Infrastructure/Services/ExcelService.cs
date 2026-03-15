// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Data;
using ClosedXML.Excel;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class ExcelService : IExcelService
{
    public ExcelService()
    {
    }

    /// <summary>
    /// Applies the header cell style.
    /// </summary>
    /// <param name="cell">The cell to style.</param>
    private void ApplyHeaderStyle(IXLCell cell)
    {
        var style = cell.Style;
        style.Fill.PatternType = XLFillPatternValues.Solid;
        style.Fill.BackgroundColor = XLColor.LightBlue;
        style.Border.BottomBorder = XLBorderStyleValues.Thin;
    }

    /// <summary>
    /// Converts a CLR value to an <see cref="XLCellValue"/> that preserves the
    /// native Excel type (Number, Boolean, DateTime, TimeSpan) instead of
    /// falling back to a string representation.
    /// </summary>
    private static XLCellValue ConvertToXLCellValue(object? value)
    {
        return value switch
        {
            null => Blank.Value,
            bool b => b,
            DateTime dt => dt,
            DateTimeOffset dto => dto.DateTime,
            TimeSpan ts => ts,
            byte n => n,
            sbyte n => n,
            short n => n,
            ushort n => n,
            int n => n,
            uint n => n,
            long n => n,
            ulong n => (double)n,
            float n => n,
            double n => n,
            decimal n => (double)n,
            string s => s,
            _ => value.ToString() ?? string.Empty
        };
    }

    /// <summary>
    /// Converts an <see cref="IXLCell"/> to its native CLR value based on
    /// <see cref="XLDataType"/>, so that <see cref="DataRow"/> consumers
    /// receive properly typed values instead of plain strings.
    /// Numbers are returned as <see cref="decimal"/> to preserve precision
    /// when downstream code converts to <c>int</c>, <c>decimal</c>, etc.
    /// via <see cref="Convert.ChangeType(object, Type)"/>.
    /// </summary>
    private static object ConvertFromXLCell(IXLCell cell)
    {
        return cell.DataType switch
        {
            XLDataType.Blank => DBNull.Value,
            XLDataType.Boolean => cell.GetBoolean(),
            XLDataType.Number => (decimal)cell.GetDouble(),
            XLDataType.DateTime => cell.GetDateTime(),
            XLDataType.TimeSpan => cell.GetTimeSpan(),
            XLDataType.Text => cell.GetString(),
            _ => cell.Value.ToString()
        };
    }

    /// <summary>
    /// Saves the given workbook to a byte array.
    /// </summary>
    /// <param name="workbook">The workbook to save.</param>
    /// <returns>A byte array representing the workbook.</returns>
    private static byte[] SaveWorkbookToByteArray(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return stream.ToArray();
    }

    public Task<byte[]> CreateTemplateAsync(IEnumerable<string> fields, string sheetName = "Sheet1")
    {
        using var workbook = new XLWorkbook();
        workbook.Properties.Author = string.Empty;
        var ws = workbook.Worksheets.Add(sheetName);
        int rowIndex = 1;
        int colIndex = 1;
        foreach (var header in fields)
        {
            var cell = ws.Cell(rowIndex, colIndex++);
            ApplyHeaderStyle(cell);
            cell.Value = header;
        }

        return Task.FromResult(SaveWorkbookToByteArray(workbook));
    }

    public Task<byte[]> ExportAsync<TData>(IEnumerable<TData> data, Dictionary<string, Func<TData, object?>> mappers, string sheetName = "Sheet1")
    {
        using var workbook = new XLWorkbook();
        workbook.Properties.Author = string.Empty;
        var ws = workbook.Worksheets.Add(sheetName);
        int rowIndex = 1;
        int colIndex = 1;
        var headers = mappers.Keys.ToList();

        // Write header row
        foreach (var header in headers)
        {
            var cell = ws.Cell(rowIndex, colIndex++);
            ApplyHeaderStyle(cell);
            cell.Value = header;
        }

        // Write data rows
        var dataList = data.ToList();
        foreach (var item in dataList)
        {
            colIndex = 1;
            rowIndex++;

            foreach (var header in headers)
            {
                var value = mappers[header](item);
                var cell = ws.Cell(rowIndex, colIndex++);
                cell.Value = ConvertToXLCellValue(value);
            }
        }

        return Task.FromResult(SaveWorkbookToByteArray(workbook));
    }

    public async Task<IResult<IEnumerable<TEntity>>> ImportAsync<TEntity>(
        byte[] data,
        Dictionary<string, Func<DataRow, TEntity, object?>> mappers,
        string sheetName = "Sheet1")
    {
        using var workbook = new XLWorkbook(new MemoryStream(data));
        if (!workbook.Worksheets.TryGetWorksheet(sheetName, out var ws))
        {
            ws = workbook.Worksheets.Count > 0 ? workbook.Worksheets.First() : null;
            if (ws is null)
            {
                return await Result<IEnumerable<TEntity>>.FailureAsync("Workbook contains no worksheets.");
            }
        }

        // Check if the worksheet contains any cells.
        var lastCellUsed = ws.LastCellUsed()?.Address.ColumnNumber ?? 0;
        if (lastCellUsed == 0)
        {
            var msg = $"Sheet with name {sheetName} is empty!";
            return await Result<IEnumerable<TEntity>>.FailureAsync(msg);
        }

        // Create a DataTable from the header row.
        var dt = new DataTable();
        bool titlesInFirstRow = true;
        foreach (var cell in ws.Range(1, 1, 1, lastCellUsed).Cells())
        {
            string colName = titlesInFirstRow ? cell.GetString() : $"Column {cell.Address.ColumnNumber}";
            dt.Columns.Add(colName, typeof(object));
        }
        int startRow = titlesInFirstRow ? 2 : 1;

        // Validate that all expected headers exist.
        var headers = mappers.Keys.ToList();
        var errors = new List<string>();
        foreach (var header in headers)
        {
            if (!dt.Columns.Contains(header))
            {
                errors.Add($"Header '{header}' does not exist in table!");
            }
        }
        if (errors.Any())
        {
            return await Result<IEnumerable<TEntity>>.FailureAsync(errors.ToArray());
        }

        var lastRowNumber = ws.LastRowUsed()?.RowNumber() ?? 0;
        var list = new List<TEntity>();

        // Process each row in the worksheet.
        for (int rowIndex = startRow; rowIndex <= lastRowNumber; rowIndex++)
        {
            var row = ws.Row(rowIndex);
            try
            {
                var dataRow = dt.NewRow();
                // Populate the DataRow with cell values, preserving native types.
                foreach (var cell in row.Cells(1, dt.Columns.Count))
                {
                    int colIndex = cell.Address.ColumnNumber - 1;
                    dataRow[colIndex] = ConvertFromXLCell(cell);
                }
                dt.Rows.Add(dataRow);

                // Create an instance of TEntity and apply the mapping functions.
                var item = (TEntity)Activator.CreateInstance(typeof(TEntity))!; // using null-forgiving operator
                foreach (var header in headers)
                {
                    mappers[header](dataRow, item);
                }
                list.Add(item);
            }
            catch (Exception e)
            {
                var errorMsg = $"Error in sheet {sheetName}: {e.Message}";
                return await Result<IEnumerable<TEntity>>.FailureAsync(errorMsg);
            }
        }

        return await Result<IEnumerable<TEntity>>.SuccessAsync(list);
    }
}

