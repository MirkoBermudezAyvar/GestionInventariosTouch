using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Domain.Entities;

namespace InventorySystem.Infrastructure.Services;

public class PdfReportGenerator : IPdfReportGenerator
{
    public PdfReportGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateLowStockReport(IEnumerable<Product> products, string generatedBy)
    {
        var productList = products.ToList();
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Text("Reporte de Inventario Bajo")
                            .SemiBold().FontSize(24).FontColor(Colors.Blue.Medium);
                        
                        column.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(10).FontColor(Colors.Grey.Medium);
                        
                        column.Item().Text($"Por: {generatedBy}")
                            .FontSize(10).FontColor(Colors.Grey.Medium);
                        
                        column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                page.Content()
                    .PaddingVertical(10)
                    .Column(column =>
                    {
                        column.Item().Text($"Total de productos con stock bajo: {productList.Count}")
                            .SemiBold().FontSize(12);

                        column.Item().PaddingTop(15);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1.5f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Medium).Padding(5)
                                    .Text("Producto").FontColor(Colors.White).SemiBold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(5)
                                    .Text("Categoría").FontColor(Colors.White).SemiBold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(5)
                                    .Text("Stock").FontColor(Colors.White).SemiBold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(5)
                                    .Text("Precio").FontColor(Colors.White).SemiBold();
                            });

                            foreach (var product in productList)
                            {
                                var backgroundColor = product.StockQuantity == 0 
                                    ? Colors.Red.Lighten4 
                                    : product.StockQuantity < 3 
                                        ? Colors.Orange.Lighten4 
                                        : Colors.Yellow.Lighten4;

                                table.Cell().Background(backgroundColor).Padding(5).Text(product.Name);
                                table.Cell().Background(backgroundColor).Padding(5).Text(product.CategoryId ?? "Sin categoría");
                                table.Cell().Background(backgroundColor).Padding(5)
                                    .Text(product.StockQuantity.ToString())
                                    .FontColor(product.StockQuantity == 0 ? Colors.Red.Darken2 : Colors.Black);
                                table.Cell().Background(backgroundColor).Padding(5).Text($"S/ {product.Price:N2}");
                            }
                        });

                        column.Item().PaddingTop(20);
                        column.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(summaryColumn =>
                        {
                            summaryColumn.Item().Text("Resumen").SemiBold().FontSize(14);
                            summaryColumn.Item().PaddingTop(5);
                            summaryColumn.Item().Text($"• Productos sin stock (0 unidades): {productList.Count(p => p.StockQuantity == 0)}");
                            summaryColumn.Item().Text($"• Productos críticos (1-2 unidades): {productList.Count(p => p.StockQuantity > 0 && p.StockQuantity < 3)}");
                            summaryColumn.Item().Text($"• Productos en alerta (3-4 unidades): {productList.Count(p => p.StockQuantity >= 3 && p.StockQuantity < 5)}");
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateInventoryReport(IEnumerable<Product> products, string generatedBy)
    {
        var productList = products.ToList();
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Text("Reporte General de Inventario")
                            .SemiBold().FontSize(22).FontColor(Colors.Blue.Medium);
                        
                        column.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm} | Por: {generatedBy}")
                            .FontSize(9).FontColor(Colors.Grey.Medium);
                        
                        column.Item().PaddingVertical(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                page.Content()
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2.5f);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1.2f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Medium).Padding(4)
                                .Text("Producto").FontColor(Colors.White).SemiBold();
                            header.Cell().Background(Colors.Blue.Medium).Padding(4)
                                .Text("Descripción").FontColor(Colors.White).SemiBold();
                            header.Cell().Background(Colors.Blue.Medium).Padding(4)
                                .Text("Categoría").FontColor(Colors.White).SemiBold();
                            header.Cell().Background(Colors.Blue.Medium).Padding(4)
                                .Text("Stock").FontColor(Colors.White).SemiBold();
                            header.Cell().Background(Colors.Blue.Medium).Padding(4)
                                .Text("Precio").FontColor(Colors.White).SemiBold();
                            header.Cell().Background(Colors.Blue.Medium).Padding(4)
                                .Text("Valor Total").FontColor(Colors.White).SemiBold();
                        });

                        var isAlternate = false;
                        foreach (var product in productList)
                        {
                            var bgColor = isAlternate ? Colors.Grey.Lighten4 : Colors.White;
                            
                            table.Cell().Background(bgColor).Padding(4).Text(product.Name);
                            table.Cell().Background(bgColor).Padding(4).Text(TruncateText(product.Description ?? "", 50));
                            table.Cell().Background(bgColor).Padding(4).Text(product.CategoryId ?? "-");
                            table.Cell().Background(bgColor).Padding(4).Text(product.StockQuantity.ToString());
                            table.Cell().Background(bgColor).Padding(4).Text($"S/ {product.Price:N2}");
                            table.Cell().Background(bgColor).Padding(4).Text($"S/ {(product.Price * product.StockQuantity):N2}");
                            
                            isAlternate = !isAlternate;
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateSingleProductLowStockReport(Product product)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Text("⚠️ Alerta de Stock Bajo")
                            .SemiBold().FontSize(24).FontColor(Colors.Red.Medium);
                        
                        column.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(10).FontColor(Colors.Grey.Medium);
                        
                        column.Item().PaddingVertical(10).LineHorizontal(2).LineColor(Colors.Red.Lighten2);
                    });

                page.Content()
                    .PaddingVertical(10)
                    .Column(column =>
                    {
                        column.Item().Background(Colors.Red.Lighten4).Padding(15).Column(alertBox =>
                        {
                            alertBox.Item().Text("Producto con Stock Bajo Detectado")
                                .SemiBold().FontSize(16).FontColor(Colors.Red.Darken2);
                            alertBox.Item().PaddingTop(5);
                            alertBox.Item().Text("Se requiere reabastecimiento inmediato.")
                                .FontSize(11).FontColor(Colors.Red.Darken1);
                        });

                        column.Item().PaddingTop(20);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                            });

                            void AddRow(string label, string value, bool highlight = false)
                            {
                                var bgColor = highlight ? Colors.Yellow.Lighten4 : Colors.Grey.Lighten4;
                                table.Cell().Background(Colors.Blue.Medium).Padding(8)
                                    .Text(label).FontColor(Colors.White).SemiBold();
                                table.Cell().Background(bgColor).Padding(8)
                                    .Text(value).FontColor(highlight ? Colors.Red.Darken2 : Colors.Black);
                            }

                            AddRow("Producto", product.Name);
                            AddRow("SKU", product.Sku ?? "N/A");
                            AddRow("Descripción", product.Description ?? "Sin descripción");
                            AddRow("Categoría", product.CategoryId ?? "Sin categoría");
                            AddRow("Stock Actual", $"{product.StockQuantity} unidades", true);
                            AddRow("Precio Unitario", $"S/ {product.Price:N2}");
                            AddRow("Valor en Inventario", $"S/ {(product.Price * product.StockQuantity):N2}");
                            AddRow("Última Actualización", product.UpdatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "N/A");
                        });

                        column.Item().PaddingTop(25);
                        column.Item().Background(Colors.Orange.Lighten4).Padding(12).Column(actionBox =>
                        {
                            actionBox.Item().Text("Acción Requerida").SemiBold().FontSize(14).FontColor(Colors.Orange.Darken3);
                            actionBox.Item().PaddingTop(5);
                            actionBox.Item().Text("• Verificar disponibilidad con proveedores");
                            actionBox.Item().Text("• Generar orden de compra si es necesario");
                            actionBox.Item().Text("• Actualizar inventario una vez recibido el stock");
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        col.Item().PaddingTop(5).Text("Sistema de Gestión de Inventarios - Touch Consulting")
                            .FontSize(9).FontColor(Colors.Grey.Medium);
                    });
            });
        });

        return document.GeneratePdf();
    }

    private static string TruncateText(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            return text;
        return text[..(maxLength - 3)] + "...";
    }
}
