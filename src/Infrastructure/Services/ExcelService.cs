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

    public async Task<byte[]> CreateTemplateAsync(IEnumerable<string> fields, string sheetName = "Sheet1")
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
                var fill = cell.Style.Fill;
                fill.PatternType = XLFillPatternValues.Solid;
                fill.SetBackgroundColor(XLColor.LightBlue);
                var border = cell.Style.Border;
                border.BottomBorder =
                    border.BottomBorder =
                        border.BottomBorder =
                            border.BottomBorder = XLBorderStyleValues.Thin;

                cell.Value = header;

                colIndex++;
            }
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                //var base64 = Convert.ToBase64String(stream.ToArray());
                stream.Seek(0, SeekOrigin.Begin);
                return await Task.FromResult(stream.ToArray());
            }
        }
    }

    public async Task<byte[]> ExportAsync<TData>(IEnumerable<TData> data
        , Dictionary<string, Func<TData, object?>> mappers
        , string sheetName = "Sheet1")
    {
        using (var workbook = new XLWorkbook())
        {
            workbook.Properties.Author = "";
            var ws = workbook.Worksheets.Add(sheetName);
            var colIndex = 1;
            var rowIndex = 1;
            var headers = mappers.Keys.Select(x => x).ToList();
            foreach (var header in headers)
            {
                var cell = ws.Cell(rowIndex, colIndex);
                var fill = cell.Style.Fill;
                fill.PatternType = XLFillPatternValues.Solid;
                fill.SetBackgroundColor(XLColor.LightBlue);
                var border = cell.Style.Border;
                border.BottomBorder =
                    border.BottomBorder =
                        border.BottomBorder =
                            border.BottomBorder = XLBorderStyleValues.Thin;

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
                    ws.Cell(rowIndex, colIndex++).Value = value==null?Blank.Value:value.ToString();
                }
            }
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                //var base64 = Convert.ToBase64String(stream.ToArray());
                stream.Seek(0, SeekOrigin.Begin);
                return await Task.FromResult(stream.ToArray());
            }
        }
    }

    public async Task<IResult<IEnumerable<TEntity>>> ImportAsync<TEntity>(byte[] data, Dictionary<string, Func<DataRow, TEntity, object?>> mappers, string sheetName = "Sheet1")
    {

        using (var workbook = new XLWorkbook(new MemoryStream(data)))
        {
            if (!workbook.Worksheets.Contains(sheetName))
            {
                return await Result<IEnumerable<TEntity>>.FailureAsync(new string[] { string.Format(_localizer["Sheet with name {0} does not exist!"], sheetName) });
            }
            var ws = workbook.Worksheet(sheetName);
            var dt = new DataTable();
            var titlesInFirstRow = true;

            foreach (var firstRowCell in ws.Range(1, 1, 1, ws.LastCellUsed().Address.ColumnNumber).Cells())
            {
                dt.Columns.Add(titlesInFirstRow ? firstRowCell.GetString() : $"Column {firstRowCell.Address.ColumnNumber}");
            }
            var startRow = titlesInFirstRow ? 2 : 1;
            var headers = mappers.Keys.Select(x => x).ToList();
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
                return await Result<IEnumerable<TEntity>>.FailureAsync(errors);
            }
            var lastRow = ws.LastRowUsed();
            var list = new List<TEntity>();
            foreach (IXLRow row in ws.Rows(startRow, lastRow.RowNumber()))
            {
                try
                {
                    DataRow dataRow = dt.Rows.Add();
                    var item = (TEntity?)Activator.CreateInstance(typeof(TEntity))??throw new NullReferenceException($"{nameof(TEntity)}");
                    foreach (IXLCell cell in row.Cells())
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
                    headers.ForEach(x => mappers[x](dataRow, item));
                    list.Add(item);
                }
                catch (Exception e)
                {
                    return await Result<IEnumerable<TEntity>>.FailureAsync(new string[] { string.Format(_localizer["Sheet name {0}:{1}"], sheetName, e.Message) });
                }
            }


            return await Result<IEnumerable<TEntity>>.SuccessAsync(list);
        }
    }
}
