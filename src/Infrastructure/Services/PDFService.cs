using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class PDFService : IPDFService
{
    private const int MarginPTs = 56;
    private const string FontFamilyName = Fonts.Arial;
    private const string ChFontFamilyName = "Noto Sans CJK SC";
    private const float FontSize = 10F;
    private const int MaxCharsPerCell = 80;
    private const int MinCharsPerCell = 10;
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
                    page.Size(landscape ? PageSizes.A4.Landscape() : PageSizes.A4);
                    page.Margin(MarginPTs);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x =>
                        x.FontSize(FontSize).FontFamily(FontFamilyName)
                            .Fallback(TextStyle.Default.FontFamily(ChFontFamilyName)));

                    page.Header()
                        .Text(title)
                        .SemiBold().FontSize(16).FontColor(Colors.Black);

                    page.Content()
                        .PaddingVertical(5, Unit.Millimetre)
                        .Table(table =>
                        {
                            var headers = mappers.Keys.Select(x => x).ToList();
                            var dataList = data.ToList();

                            // Rough fit columns calculation
                            var tableWidth = landscape
                                ? (int)(PageSizes.A4.Landscape().Width - MarginPTs * 2)
                                : (int)(PageSizes.A4.Width - MarginPTs * 2);
                            var columnsWidth = new int[headers.Count];

                            for (uint c = 0; c < headers.Count; c++)
                            {
                                var cellWidth = Math.Max(MinCharsPerCell,
                                    Math.Min($"{headers[(int)c]}".Length, MaxCharsPerCell));

                                if (columnsWidth[c] < cellWidth)
                                    columnsWidth[c] = cellWidth;
                            }

                            foreach (var item in dataList)
                            {
                                var result = headers.Select(header => mappers[header](item));

                                uint c = 0;
                                foreach (var value in result)
                                {
                                    var cellWidth = Math.Max(MinCharsPerCell,
                                        Math.Min($"{value}".Length, MaxCharsPerCell));
                                    if (columnsWidth[c] < cellWidth)
                                        columnsWidth[c] = cellWidth;
                                    c += 1;
                                }
                            }

                            var sumWidth = columnsWidth.Sum();
                            var ratio = tableWidth / (float)sumWidth;
                            for (var i = 0; i < columnsWidth.Length; i++)
                                columnsWidth[i] = (int)(columnsWidth[i] * ratio);

                            // Create columns
                            table.ColumnsDefinition(columns =>
                            {
                                for (uint c = 0; c < headers.Count; c++)
                                {
                                    columns.ConstantColumn(columnsWidth[c]);
                                    table.Cell().Row(1).Column(c + 1).Element(BlockHeader).Text(headers[(int)c]);
                                }
                            });

                            // Create rows
                            uint rowIndex = 1;
                            foreach (var item in dataList)
                            {
                                uint colIndex = 1;
                                rowIndex++;

                                var result = headers.Select(header => mappers[header](item));

                                foreach (var value in result)
                                {
                                    if (IsNumber(value))
                                        table.Cell().Row(rowIndex).Column(colIndex).Element(BlockCell).AlignRight()
                                            .Text($"{value}");
                                    else
                                        table.Cell().Row(rowIndex).Column(colIndex).Element(BlockCell).AlignLeft()
                                            .Text($"{value}");
                                    ;

                                    colIndex += 1;
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

    private static bool IsNumber(object? value)
    {
        if (value == null)
            return false;
        return value is sbyte or byte or short or ushort or int or uint or long or ulong or float or double or decimal;
    }

    private static IContainer BlockCell(IContainer container)
    {
        return container
            .Border(1)
            .Background(Colors.White)
            .Padding(1, Unit.Millimetre)
            .ShowOnce()
            .AlignMiddle();
    }

    private static IContainer BlockHeader(IContainer container)
    {
        return container
            .Border(1)
            .Background(Colors.Grey.Lighten3)
            .Padding(1, Unit.Millimetre)
            .AlignCenter()
            .AlignMiddle();
    }
}