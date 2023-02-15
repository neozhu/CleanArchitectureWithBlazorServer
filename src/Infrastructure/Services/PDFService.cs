using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;
using Document = QuestPDF.Fluent.Document;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class PDFService : IPDFService
{
    private const int marginPTs = 56;
    private const string fontFamilyName = Fonts.Calibri;
    private const float fontSize = 10F;
    private const int maxCharsPerCell = 80;
    private const int minCharsPerCell = 10;

    public async Task<byte[]> ExportAsync<TData>(IEnumerable<TData> data
        , Dictionary<string, Func<TData, object?>> mappers
        , string title, bool landscape)
    {
        var stream = new MemoryStream();
        //QuestPDF.Settings.DocumentLayoutExceptionThreshold = 1000;
        Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(landscape? PageSizes.A4.Landscape() : PageSizes.A4);
                        page.Margin(marginPTs, Unit.Point);
                        page.PageColor(QuestPDF.Helpers.Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(fontSize).FontFamily(fontFamilyName).Fallback(TextStyle.Default.FontFamily("Microsoft YaHei")));
                        
                        page.Header()
                            .Text(title)
                            .SemiBold().FontSize(16).FontColor(QuestPDF.Helpers.Colors.Black);

                        page.Content()
                            .PaddingVertical(5, Unit.Millimetre)
                            .Table(table =>
                            {
                                var headers = mappers.Keys.Select(x => x).ToList();
                                var dataList = data.ToList();

                                // Rough fit columns calculation
                                int tableWidth = landscape ? (int)(PageSizes.A4.Landscape().Width - (marginPTs * 2)) : (int)(PageSizes.A4.Width - (marginPTs * 2));
                                int[] columnsWidth = new int[headers.Count];

                                for (uint c = 0; c < headers.Count; c++)
                                {
                                    var cellWidth = Math.Max(minCharsPerCell, Math.Min($"{headers[(int)c]}".Length,maxCharsPerCell));

                                    if (columnsWidth[c] < cellWidth)
                                        columnsWidth[c] = cellWidth;
                                }

                                foreach (var item in dataList)
                                {
                                    var result = headers.Select(header => mappers[header](item));

                                    uint c = 0;
                                    foreach (var value in result)
                                    {
                                        var cellWidth = Math.Max(minCharsPerCell, Math.Min($"{value}".Length, maxCharsPerCell));
                                        if (columnsWidth[c] < cellWidth)
                                            columnsWidth[c] = cellWidth;
                                        c += 1;
                                    }
                                }

                                int sumWidth = columnsWidth.Sum();
                                float ratio = (float)tableWidth / (float)sumWidth;
                                for (int i = 0;i < columnsWidth.Length; i++)
                                    columnsWidth[i] = (int)(columnsWidth[i] * ratio);

                                // Create columns
                                table.ColumnsDefinition(columns =>
                                {
                                    for(uint c=0; c<headers.Count; c++)
                                    {
                                        columns.ConstantColumn(columnsWidth[c], Unit.Point);
                                        table.Cell().Row(1).Column(c+1).Element(BlockHeader).Text(headers[(int)c]);
                                    }
                                });
                                
                                // Create rows
                                uint colIndex = 1;
                                uint rowIndex = 1;
                                foreach (var item in dataList)
                                {
                                    colIndex = 1;
                                    rowIndex++;

                                    var result = headers.Select(header => mappers[header](item));

                                    foreach (var value in result)
                                    {
                                        if (IsNumber(value))
                                            table.Cell().Row(rowIndex).Column(colIndex).Element(BlockCell).AlignRight().Text($"{value}");
                                        else
                                            table.Cell().Row(rowIndex).Column(colIndex).Element(BlockCell).AlignLeft().Text($"{value}"); ;

                                        colIndex+=1;
                                    }
                                }
                            });

                        page.Footer()
                            .AlignRight()
                            .Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                                x.Span(" of ");
                                x.TotalPages();
                            });
                    });
                })
            .GeneratePdf(stream);

        return await Task.FromResult(stream.ToArray());
    }

    static bool IsNumber(object? value)
    {
        if (value == null)
            return false;
        else
            return value is sbyte
                || value is byte
                || value is short
                || value is ushort
                || value is int
                || value is uint
                || value is long
                || value is ulong
                || value is float
                || value is double
                || value is decimal;
    }

    static IContainer BlockCell(IContainer container)
    {
        return container
            .Border(1)
            .Background(QuestPDF.Helpers.Colors.White)
            .Padding(1, Unit.Millimetre)
            .ShowOnce()
            .AlignMiddle();
    }

    static IContainer BlockHeader(IContainer container)
    {
        return container
            .Border(1)
            .Background(QuestPDF.Helpers.Colors.Grey.Lighten3)
            .Padding(1, Unit.Millimetre)
            .AlignCenter()
            .AlignMiddle();
    }
}