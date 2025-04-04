using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders; // Add this line
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic; // Ensure this is included for IEnumerable
using System.IO;

namespace Ont3010_Project_YA2024.Data.InventoryLiaisonRepServices
{

    public class GenerateFridgeReportPdf
    {
        public byte[] CreateFridgeReportPdf(string startDate, string endDate, string businessName, string contactEmail, string contactPhone,
           string street, string city, string postalCode, string stateProvince, string country, string address, string businessLogoBase64, IEnumerable<FridgeReportData> reportData)
        {
            using (var memoryStream = new MemoryStream())
            {
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                document.Add(new Paragraph(businessName)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(18)
                    .SetBold());

                // Create a table with 2 columns (one for the logo and one for the address)
                Table logoAddressTable = new Table(2)
                    .SetWidth(UnitValue.CreatePercentValue(100)); // Set table width to 100% of the available space

                // Add logo to the first column (left)
                if (!string.IsNullOrEmpty(businessLogoBase64))
                {
                    try
                    {
                        byte[] imageData = Convert.FromBase64String(businessLogoBase64);
                        var image = new Image(ImageDataFactory.Create(imageData))
                            .SetWidth(80)
                            .SetHeight(75)
                            .SetHorizontalAlignment(HorizontalAlignment.LEFT); // Align logo to the left

                        // Add the image to a cell
                        Cell logoCell = new Cell().Add(image)
                            .SetBorder(Border.NO_BORDER)
                            .SetPaddingRight(10); // Add padding to separate logo and address
                        logoAddressTable.AddCell(logoCell);
                    }
                    catch (FormatException)
                    {
                        logoAddressTable.AddCell(new Cell().Add(new Paragraph("Invalid logo format."))
                            .SetBorder(Border.NO_BORDER));
                    }
                    catch (Exception ex)
                    {
                        logoAddressTable.AddCell(new Cell().Add(new Paragraph($"Error loading logo: {ex.Message}"))
                            .SetBorder(Border.NO_BORDER));
                    }
                }
                else
                {
                    logoAddressTable.AddCell(new Cell().Add(new Paragraph("No Logo Available"))
                        .SetBorder(Border.NO_BORDER));
                }

                // Prepare formatted address
                string formattedAddress = $"{contactEmail}\n{contactPhone}\n{street}\n{city}\n{postalCode}\n{stateProvince}\n{country}";

                // Add address to the second column (right)
                var addressParagraph = new Paragraph(formattedAddress)
                    .SetFontSize(12)
                    .SetTextAlignment(TextAlignment.RIGHT); // Align text to the right

                Cell addressCell = new Cell().Add(addressParagraph)
                    .SetBorder(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.RIGHT); // Ensure text is right aligned in the cell
                logoAddressTable.AddCell(addressCell);

                // Add the table containing logo and address to the document
                document.Add(logoAddressTable);

                document.Add(new Paragraph("Fridge Inventory Report")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(22)
                    .SetBold());

                // Add report date range
                document.Add(new Paragraph($"Report Date Range: {FormatDate(startDate)} to {FormatDate(endDate)}")
                   .SetTextAlignment(TextAlignment.LEFT)
                   .SetFontSize(12)
                   .SetMarginTop(5));
                document.Add(new Paragraph($"Generated On: {@DateTime.Now.ToString("MMMM dd, yyyy HH:mm")}")
                      .SetTextAlignment(TextAlignment.LEFT)
                      .SetMarginTop(20)
                      .SetFontSize(10));

                // Add a table for report data
                if (reportData != null && reportData.Any())
                {
                    var table = new Table(6) // Adjust the number of columns to match your data
                        .SetWidth(UnitValue.CreatePercentValue(100))
                        .SetMarginTop(10)
                        .SetBorder(Border.NO_BORDER); // Remove table borders

                    // Adding headers
                    table.AddHeaderCell("Fridge ID").SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    table.AddHeaderCell("Serial Number").SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    table.AddHeaderCell("Model Type").SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    table.AddHeaderCell("Condition").SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    table.AddHeaderCell("Capacity").SetBackgroundColor(ColorConstants.LIGHT_GRAY); // New header
                    table.AddHeaderCell("Supplier Name").SetBackgroundColor(ColorConstants.LIGHT_GRAY); // New header

                    int index = 0;
                    foreach (var data in reportData)
                    {
                        // Alternate row colors
                        var rowColor = index % 2 == 0 ? ColorConstants.WHITE : ColorConstants.LIGHT_GRAY;

                        table.AddCell(data.FridgeId.ToString()).SetBackgroundColor(rowColor);
                        table.AddCell(data.SerialNumber).SetBackgroundColor(rowColor);
                        table.AddCell(data.ModelType).SetBackgroundColor(rowColor);
                        table.AddCell(data.Condition).SetBackgroundColor(rowColor);
                        table.AddCell(data.Capacity).SetBackgroundColor(rowColor); // New cell
                        table.AddCell(data.SupplierName).SetBackgroundColor(rowColor); // New cell

                        index++;
                    }

                    document.Add(table);
                    document.Add(new Paragraph($"Total Fridges: {reportData.Count()}").SetTextAlignment(TextAlignment.RIGHT).SetMarginTop(10));
                }
                else
                {
                    document.Add(new Paragraph("No data available for the selected date range.")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(12)
                        .SetMarginTop(10)
                        .SetItalic());
                }

                document.Add(new Paragraph($"Generated by: {businessName}")
                        .SetTextAlignment(TextAlignment.RIGHT)
                        .SetMarginTop(20)
                        .SetFontSize(10));
                // Add footer Generated On: @DateTime.Now.ToString("MMMM dd, yyyy HH:mm")
              
                document.Close();
                return memoryStream.ToArray();
            }
        }

        internal async Task<IActionResult>? DownloadChartPdf(string chartData)
        {
            throw new NotImplementedException();
        }

        private string FormatDate(string dateString)
        {
            if (DateTime.TryParse(dateString, out DateTime date))
            {
                return date.ToString("MMMM dd, yyyy");
            }
            return dateString; // Return original if parsing fails
        }

        public byte[] CreateFridgeReportPdfWithChart(
            string startDate,
            string endDate,
            string businessName,
            string contactEmail,
            byte[] chartImage, // Chart image bytes
            List<FridgeReportData> reportData)
        {
            using (var ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf, PageSize.A4);
                document.SetMargins(20, 20, 20, 20);

                // Add business and report details
                document.Add(new Paragraph($"Business Name: {businessName}"));
                document.Add(new Paragraph($"Contact Email: {contactEmail}"));
                document.Add(new Paragraph($"Report Date Range: {startDate} - {endDate}"));

                // Embed the chart image in the PDF
                if (chartImage != null)
                {
                    ImageData imageData = ImageDataFactory.Create(chartImage);
                    iText.Layout.Element.Image chart = new iText.Layout.Element.Image(imageData);
                    chart.SetAutoScale(true);
                    document.Add(chart); // Add chart image to the PDF
                }

                // Add other report content here (e.g., tables, text)

                document.Close();
                return ms.ToArray();
            }
        }


    }
}