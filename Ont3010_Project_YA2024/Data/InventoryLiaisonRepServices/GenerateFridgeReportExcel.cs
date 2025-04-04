using OfficeOpenXml;
using OfficeOpenXml.Style; // For styling
using System.Collections.Generic;
using System.IO;

namespace Ont3010_Project_YA2024.Data.InventoryLiaisonRepServices
{
    public class GenerateFridgeReportExcel
    {
        public byte[] CreateFridgeReportExcel(string startDate, string endDate, IEnumerable<FridgeReportData> reportData)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var package = new ExcelPackage(memoryStream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Fridge Report");

                    // Set title and date range
                    worksheet.Cells["A1"].Value = "Fridge Report";
                    worksheet.Cells["A2"].Value = $"From: {startDate} To: {endDate}";

                    // Add column headers
                    worksheet.Cells["A4"].Value = "Fridge ID";
                    worksheet.Cells["B4"].Value = "Serial Number";
                    worksheet.Cells["C4"].Value = "Model Type";
                    worksheet.Cells["D4"].Value = "Condition";
                    worksheet.Cells["E4"].Value = "Door Type"; // New header
                    worksheet.Cells["F4"].Value = "Size"; // New header
                    worksheet.Cells["G4"].Value = "Capacity"; // New header
                    worksheet.Cells["H4"].Value = "Supplier Name"; // New header
                    worksheet.Cells["I4"].Value = "Supplier Contact"; // New header
                    worksheet.Cells["J4"].Value = "In Stock"; // New header
                    worksheet.Cells["K4"].Value = "Warranty End Date"; // New header

                    // Apply header styling
                    using (var range = worksheet.Cells["A4:K4"])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    // Add data rows and count fridges
                    int row = 5; // Start adding data from row 5
                    int fridgeCount = 0; // Count of fridges
                    if (reportData != null)
                    {
                        foreach (var data in reportData)
                        {
                            worksheet.Cells[row, 1].Value = data.FridgeId;
                            worksheet.Cells[row, 2].Value = data.SerialNumber;
                            worksheet.Cells[row, 3].Value = data.ModelType;
                            worksheet.Cells[row, 4].Value = data.Condition;
                            worksheet.Cells[row, 5].Value = data.DoorType; // New data
                            worksheet.Cells[row, 6].Value = data.Size; // New data
                            worksheet.Cells[row, 7].Value = data.Capacity; // New data
                            worksheet.Cells[row, 8].Value = data.SupplierName; // New data
                            worksheet.Cells[row, 9].Value = data.SupplierContact; // New data
                            worksheet.Cells[row, 10].Value = data.IsInStock ? "Yes" : "No"; // New data
                            worksheet.Cells[row, 11].Value = data.WarrantyEndDate?.ToString("d"); // New data
                            row++;
                            fridgeCount++;
                        }
                    }
                    else
                    {
                        worksheet.Cells[row, 1].Value = "No data available for the selected date range.";
                    }
                    worksheet.Cells[row + 1, 1].Value = "Total Fridges:";
                    worksheet.Cells[row + 1, 2].Value = fridgeCount;

                    // Apply styling for the total count
                    worksheet.Cells[row + 1, 1, row + 1, 2].Style.Font.Bold = true;
                    // Auto fit columns
                    worksheet.Cells.AutoFitColumns();

                    // Save the package
                    package.Save();
                }
                return memoryStream.ToArray();
            }
        }
    }

}
