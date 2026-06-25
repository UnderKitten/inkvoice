using Inkvoice.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QpdfDocument = QuestPDF.Fluent.Document;

namespace Inkvoice.Services;

public static class InvoicePdf
{
    public static byte[] Generate(Models.Document doc, Business biz)
    {
        return QpdfDocument
            .Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // ---------- HEADER: you (left) + doc title (right) ----------
                    page.Header()
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .Column(col =>
                                {
                                    col.Item().Text(biz.Name).FontSize(16).Bold();
                                    col.Item().Text(biz.AddressLine1);
                                    if (!string.IsNullOrWhiteSpace(biz.AddressLine2))
                                        col.Item().Text(biz.AddressLine2);
                                    col.Item()
                                        .Text($"{biz.City}, {biz.Province}  {biz.PostalCode}");
                                    if (!string.IsNullOrWhiteSpace(biz.Email))
                                        col.Item().Text(biz.Email);
                                    if (!string.IsNullOrWhiteSpace(biz.Phone))
                                        col.Item().Text(biz.Phone);
                                });

                            row.RelativeItem()
                                .AlignRight()
                                .Column(col =>
                                {
                                    col.Item()
                                        .Text(doc.Type.ToString().ToUpper())
                                        .FontSize(20)
                                        .Bold();
                                    col.Item().Text($"# {doc.Number}");
                                    col.Item().Text($"Date: {doc.IssueDate:yyyy-MM-dd}");
                                    if (doc.DueDate is not null)
                                        col.Item().Text($"Due: {doc.DueDate:yyyy-MM-dd}");
                                });
                        });

                    // ---------- CONTENT ----------
                    page.Content()
                        .PaddingVertical(20)
                        .Column(col =>
                        {
                            // Bill To
                            col.Item()
                                .PaddingBottom(10)
                                .Column(c =>
                                {
                                    c.Item().Text("Bill To:").Bold();
                                    c.Item().Text(doc.Client?.Name ?? "");
                                    if (doc.Client is not null)
                                    {
                                        c.Item().Text(doc.Client.AddressLine1);
                                        c.Item()
                                            .Text(
                                                $"{doc.Client.City}, {doc.Client.Province}  {doc.Client.PostalCode}"
                                            );
                                    }
                                });

                            // Line items table
                            col.Item()
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2); // Item
                                        columns.RelativeColumn(4); // Description
                                        columns.RelativeColumn(1); // Hours
                                        columns.RelativeColumn(1); // Rate
                                        columns.RelativeColumn(1.3f); // Amount
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(HeaderCell).Text("Item");
                                        header.Cell().Element(HeaderCell).Text("Description");
                                        header
                                            .Cell()
                                            .Element(HeaderCell)
                                            .AlignRight()
                                            .Text("Hours");
                                        header.Cell().Element(HeaderCell).AlignRight().Text("Rate");
                                        header
                                            .Cell()
                                            .Element(HeaderCell)
                                            .AlignRight()
                                            .Text("Amount");
                                    });

                                    foreach (var item in doc.Items)
                                    {
                                        table.Cell().Element(BodyCell).Text(item.ItemName);
                                        table.Cell().Element(BodyCell).Text(item.Description);
                                        table
                                            .Cell()
                                            .Element(BodyCell)
                                            .AlignRight()
                                            .Text(FormatHours(item.Hours));
                                        table
                                            .Cell()
                                            .Element(BodyCell)
                                            .AlignRight()
                                            .Text(item.Rate.ToString("C"));
                                        table
                                            .Cell()
                                            .Element(BodyCell)
                                            .AlignRight()
                                            .Text(item.RowTotal.ToString("C"));
                                    }
                                });

                            // Totals
                            col.Item()
                                .AlignRight()
                                .PaddingTop(10)
                                .Column(c =>
                                {
                                    c.Item().Text($"Subtotal: {doc.Subtotal:C}");
                                    c.Item().Text($"HST ({doc.TaxRate * 100:0.##}%): {doc.TaxAmount:C}");
                                    c.Item().Text($"Total: {doc.Total:C}").Bold().FontSize(12);
                                });

                            // Comments
                            if (!string.IsNullOrWhiteSpace(doc.Comments))
                            {
                                col.Item()
                                    .PaddingTop(20)
                                    .Column(c =>
                                    {
                                        c.Item().Text("Notes:").Bold();
                                        c.Item().Text(doc.Comments);
                                    });
                            }
                        });

                    // ---------- FOOTER ----------
                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            if (!string.IsNullOrWhiteSpace(biz.GstHstNumber))
                                text.Span($"GST/HST #: {biz.GstHstNumber}");
                        });
                });
            })
            .GeneratePdf();

        // local helpers for consistent cell styling
        static IContainer HeaderCell(IContainer c) =>
            c.BorderBottom(1).PaddingVertical(4).DefaultTextStyle(x => x.Bold());
        static IContainer BodyCell(IContainer c) =>
            c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4);

        // 30 -> "30:00h", 30.5 -> "30:30h"
        static string FormatHours(decimal hours)
        {
            int h = (int)hours;
            int m = (int)Math.Round((hours - h) * 60);
            return $"{h}:{m:00}h";
        }
    }
}
