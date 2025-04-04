using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Ont3010_Project_YA2024.Models.CustomerLiaison;
using System.IO;
using System.Threading.Tasks;


namespace Ont3010_Project_YA2024.Data.CustomerLiaisonRepServices
{
    public class PdfService
    {
        public async Task<byte[]> GenerateCustomerReportPdfAsync(IEnumerable<Customer> customers, string startDate, string endDate, string businessName, string contactEmail, string contactPhone,
           string street, string city, string postalCode, string stateProvince, string country, string address, string businessLogoBase64)
        {
            using (var memoryStream = new MemoryStream())
            {
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Add Business Name
                document.Add(new Paragraph(businessName)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(18)
                    .SetBold());

                // Create a table for logo and business details
                Table logoAddressTable = new Table(2).SetWidth(UnitValue.CreatePercentValue(100));

                // Add logo to the first column
                if (!string.IsNullOrEmpty(businessLogoBase64))
                {
                    try
                    {
                        byte[] logoBytes = Convert.FromBase64String(businessLogoBase64);
                        var logo = new Image(ImageDataFactory.Create(logoBytes))
                            .SetWidth(80)
                            .SetHeight(75)
                            .SetHorizontalAlignment(HorizontalAlignment.LEFT);

                        Cell logoCell = new Cell().Add(logo)
                            .SetBorder(Border.NO_BORDER)
                            .SetPaddingRight(10);
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
                string formattedAddress = $"{contactEmail}\n{contactPhone}\n{street}\n{city}\n{postalCode}\n{stateProvince}\n{country}";
                // Prepare formatted address
                var addressParagraph = new Paragraph(formattedAddress)
                    .SetFontSize(12)
                    .SetTextAlignment(TextAlignment.RIGHT);
                Cell addressCell = new Cell().Add(addressParagraph)
                    .SetBorder(Border.NO_BORDER);
                logoAddressTable.AddCell(addressCell);

                // Add the table containing logo and address to the document
                document.Add(logoAddressTable);

                // Add Report Title
                document.Add(new Paragraph("Customer Detailed Report")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(22)
                    .SetBold());
                document.Add(new Paragraph($"Report Date Range: {FormatDate(startDate)} to {FormatDate(endDate)}")
                  .SetTextAlignment(TextAlignment.LEFT)
                  .SetFontSize(12)
                  .SetMarginTop(5));
                // Add Report Date
                document.Add(new Paragraph($"Report Date: {DateTime.Now:dd MMMM yyyy}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(12)
                    .SetMarginTop(5));

                // Add Table for Customer Data
                var table = new Table(new float[] { 0.8f, 2f, 2f, 2f, 1.5f, 1.5f, 2f, 1.5f, 1.5f, 1.5f })
                .SetWidth(UnitValue.CreatePercentValue(100))
                .SetMarginTop(5) // Reduced margin
                .SetPadding(2); // Reduced padding for the cells

                // Adding headers with smaller font size
                table.AddHeaderCell("Customer ID").SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetFontSize(6);
                table.AddHeaderCell("First Name").SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetFontSize(6);
                table.AddHeaderCell("Last Name").SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetFontSize(6);
                table.AddHeaderCell("Email Address").SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetFontSize(6);
                table.AddHeaderCell("Phone Number").SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetFontSize(6);
                table.AddHeaderCell("Business Name").SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetFontSize(6);
                table.AddHeaderCell("Business Role").SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetFontSize(6);
                table.AddHeaderCell("City").SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetFontSize(6);
                table.AddHeaderCell("Country").SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetFontSize(6);
                table.AddHeaderCell("Created Date").SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetFontSize(6);

                foreach (var customer in customers)
                {
                    table.AddCell(customer.CustomerId.ToString()).SetPadding(2); // Reduced padding
                    table.AddCell(customer.FirstName).SetPadding(2);
                    table.AddCell(customer.LastName).SetPadding(2);
                    table.AddCell(customer.EmailAddress).SetPadding(2);
                    table.AddCell(customer.PhoneNumber).SetPadding(2);
                    table.AddCell(customer.BusinessName).SetPadding(2);
                    table.AddCell(customer.BusinessRole).SetPadding(2);
                    table.AddCell(customer.City).SetPadding(2);
                    table.AddCell(customer.Country).SetPadding(2);
                    table.AddCell(customer.CreatedDate.ToShortDateString()).SetPadding(2);
                }


                document.Add(table);
                document.Add(new Paragraph($"Total Customers: {customers.Count()}").SetTextAlignment(TextAlignment.RIGHT).SetMarginTop(10));

                document.Add(new Paragraph($"Generated by: {businessName}")
                      .SetTextAlignment(TextAlignment.RIGHT)
                      .SetMarginTop(20)
                      .SetFontSize(10));
                document.Close();

                return memoryStream.ToArray();
            }


        }

        private string FormatDate(string dateString)
        {
            if (DateTime.TryParse(dateString, out DateTime date))
            {
                return date.ToString("MMMM dd, yyyy");
            }
            return dateString; // Return original if parsing fails
        }
    }
}


