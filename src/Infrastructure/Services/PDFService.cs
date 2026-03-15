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

    public Task<byte[]> ExportAsync<TData>(IEnumerable<TData> data
        , Dictionary<string, Func<TData, object?>> mappers
        , string title, bool landscape)
    {
        using var stream = new MemoryStream();
        //QuestPDF.Settings.DocumentLayoutExceptionThreshold = 1000;
        Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(landscape ? PageSizes.A4.Landscape() : PageSizes.A4);
                    page.Margin(MarginPTs);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x =>
                        x.FontSize(FontSize).FontFamily(FontFamilyName));

                    page.Header()
                        .Text(title)
                        .SemiBold().FontSize(16).FontColor(Colors.Black);

                    page.Content()
                        .PaddingVertical(5, Unit.Millimetre)
                        .Table(table =>
                        {
                            var headers = mappers.Keys.ToList();
                            var dataList = data.ToList();

                            // Pre-evaluate all mapper values to avoid calling mappers twice
                            var allValues = dataList
                                .Select(item => headers.Select(header => mappers[header](item)).ToList())
                                .ToList();

                            // Rough fit columns calculation
                            var tableWidth = landscape
                                ? (int)(PageSizes.A4.Landscape().Width - MarginPTs * 2)
                                : (int)(PageSizes.A4.Width - MarginPTs * 2);
                            var columnsWidth = new int[headers.Count];

                            for (var c = 0; c < headers.Count; c++)
                            {
                                columnsWidth[c] = Math.Max(MinCharsPerCell,
                                    Math.Min(headers[c].Length, MaxCharsPerCell));
                            }

                            foreach (var row in allValues)
                            {
                                for (var c = 0; c < row.Count; c++)
                                {
                                    var cellWidth = Math.Max(MinCharsPerCell,
                                        Math.Min($"{row[c]}".Length, MaxCharsPerCell));
                                    if (columnsWidth[c] < cellWidth)
                                        columnsWidth[c] = cellWidth;
                                }
                            }

                            var sumWidth = columnsWidth.Sum();
                            var ratio = tableWidth / (float)sumWidth;
                            for (var i = 0; i < columnsWidth.Length; i++)
                                columnsWidth[i] = (int)(columnsWidth[i] * ratio);

                            // Define columns
                            table.ColumnsDefinition(columns =>
                            {
                                for (var c = 0; c < headers.Count; c++)
                                    columns.ConstantColumn(columnsWidth[c]);
                            });

                            // Create header row
                            for (uint c = 0; c < headers.Count; c++)
                            {
                                table.Cell().Row(1).Column(c + 1).Element(BlockHeader).Text(headers[(int)c]);
                            }

                            // Create data rows
                            uint rowIndex = 1;
                            foreach (var row in allValues)
                            {
                                uint colIndex = 1;
                                rowIndex++;

                                foreach (var value in row)
                                {
                                    var cell = table.Cell().Row(rowIndex).Column(colIndex);
                                    if (IsNumber(value))
                                        cell.Element(BlockCell).AlignRight().Text($"{value}");
                                    else
                                        cell.Element(BlockCell).AlignLeft().Text($"{value}");

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

        return Task.FromResult(stream.ToArray());
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
