// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Data;
using ClosedXML.Excel;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class ExcelService : IExcelService
{
    private readonly IStringLocalizer<ExcelService> _localizer;

    public ExcelService(IStringLocalizer<ExcelService> localizer)
    {
        _localizer = localizer;
    }

    public Task<byte[]> CreateTemplateAsync(IEnumerable<string> fields, string sheetName = "Sheet1")
    {
        using (var workbook = new XLWorkbook())
        {
            workbook.Properties.Author = "";
            var ws = workbook.Worksheets.Add(sheetName);
            var colIndex = 1;
            var rowIndex = 1;
            foreach (var header in fields)
            {
                var cell = ws.Cell(rowIndex, colIndex);
                var style = cell.Style;
                style.Fill.PatternType = XLFillPatternValues.Solid;
                style.Fill.BackgroundColor = XLColor.LightBlue;
                style.Border.BottomBorder = XLBorderStyleValues.Thin;

                cell.Value = header;

                colIndex++;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return Task.FromResult(stream.ToArray());
            }
        }
    }

    public Task<byte[]> ExportAsync<TData>(IEnumerable<TData> data, Dictionary<string, Func<TData, object?>> mappers, string sheetName = "Sheet1")
    {
        using (var workbook = new XLWorkbook())
        {
            workbook.Properties.Author = "";
            var ws = workbook.Worksheets.Add(sheetName);
            var colIndex = 1;
            var rowIndex = 1;
            var headers = mappers.Keys.ToList();
            foreach (var header in headers)
            {
                var cell = ws.Cell(rowIndex, colIndex);
                var style = cell.Style;
                style.Fill.PatternType = XLFillPatternValues.Solid;
                style.Fill.BackgroundColor = XLColor.LightBlue;
                style.Border.BottomBorder = XLBorderStyleValues.Thin;

                cell.Value = header;

                colIndex++;
            }

            var dataList = data.ToList();
            foreach (var item in dataList)
            {
                colIndex = 1;
                rowIndex++;

                var result = headers.Select(header => mappers[header](item));

                foreach (var value in result)
                {
                    ws.Cell(rowIndex, colIndex).Value = value == null ? Blank.Value : value.ToString();
                    colIndex++;
                }
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return Task.FromResult(stream.ToArray());
            }
        }
    }

    public async Task<IResult<IEnumerable<TEntity>>> ImportAsync<TEntity>(byte[] data, Dictionary<string, Func<DataRow, TEntity, object?>> mappers, string sheetName = "Sheet1")
    {
        using (var workbook = new XLWorkbook(new MemoryStream(data)))
        {
            if (!workbook.Worksheets.TryGetWorksheet(sheetName, out var ws))
            {
                return await Result<IEnumerable<TEntity>>.FailureAsync(string.Format(_localizer["Sheet with name {0} does not exist!"], sheetName));
            }

            var dt = new DataTable();
            var titlesInFirstRow = true;

            foreach (var firstRowCell in ws.Range(1, 1, 1, ws.LastCellUsed().Address.ColumnNumber).Cells())
            {
                dt.Columns.Add(titlesInFirstRow ? firstRowCell.GetString() : $"Column {firstRowCell.Address.ColumnNumber}");
            }

            var startRow = titlesInFirstRow ? 2 : 1;
            var headers = mappers.Keys.ToList();
            var errors = new List<string>();

            foreach (var header in headers)
            {
                if (!dt.Columns.Contains(header))
                {
                    errors.Add(string.Format(_localizer["Header '{0}' does not exist in table!"], header));
                }
            }

            if (errors.Any())
            {
                return await Result<IEnumerable<TEntity>>.FailureAsync(errors.ToArray());
            }

            var lastRow = ws.LastRowUsed();
            var list = new List<TEntity>();

            foreach (var row in ws.Rows(startRow, lastRow.RowNumber()))
            {
                try
                {
                    var dataRow = dt.Rows.Add();
                    var item = (TEntity?)Activator.CreateInstance(typeof(TEntity)) ?? throw new NullReferenceException($"{nameof(TEntity)}");

                    foreach (var cell in row.Cells())
                    {
                        if (cell.DataType == XLDataType.DateTime)
                        {
                            dataRow[cell.Address.ColumnNumber - 1] = cell.GetDateTime().ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            dataRow[cell.Address.ColumnNumber - 1] = cell.Value.ToString();
                        }
                    }

                    foreach (var header in headers)
                    {
                        mappers[header](dataRow, item);
                    }

                    list.Add(item);
                }
                catch (Exception e)
                {
                    return await Result<IEnumerable<TEntity>>.FailureAsync(string.Format(_localizer["Sheet name {0}:{1}"], sheetName, e.Message));
                }
            }

            return await Result<IEnumerable<TEntity>>.SuccessAsync(list);
        }
    }
}
