using OfficeOpenXml;
using Ont3010_Project_YA2024.Models.CustomerLiaison;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace Ont3010_Project_YA2024.Data.CustomerLiaisonRepServices
{
    public class ExcelService
    {
       
        public async Task<byte[]> GenerateCustomerReportExcelAsync(IEnumerable<Customer> customers)
        {
            using(var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Customer Report");

                // Add header
                worksheet.Cells[1, 1].Value = "Customer ID";
                worksheet.Cells[1, 2].Value = "First Name";
                worksheet.Cells[1, 3].Value = "Last Name";
                worksheet.Cells[1, 4].Value = "Email Address";
                worksheet.Cells[1, 5].Value = "Phone Number";
                worksheet.Cells[1, 6].Value = "Business Name";
                worksheet.Cells[1, 7].Value = "Business Role";
                worksheet.Cells[1, 8].Value = "City";
                worksheet.Cells[1, 9].Value = "Country";
                worksheet.Cells[1, 10].Value = "Created Date";

                int row = 2;
                int CustomerCount = 0;
                foreach (var customer in customers)
                {
                    worksheet.Cells[row, 1].Value = customer.CustomerId;
                    worksheet.Cells[row, 2].Value = customer.FirstName;
                    worksheet.Cells[row, 3].Value = customer.LastName;
                    worksheet.Cells[row, 4].Value = customer.EmailAddress;
                    worksheet.Cells[row, 5].Value = customer.PhoneNumber;
                    worksheet.Cells[row, 6].Value = customer.BusinessName;
                    worksheet.Cells[row, 7].Value = customer.BusinessRole;
                    worksheet.Cells[row, 8].Value = customer.City;
                    worksheet.Cells[row, 9].Value = customer.Country;
                    worksheet.Cells[row, 10].Value = customer.CreatedDate.ToShortDateString();

                    row++;
                    CustomerCount++;
                }
                worksheet.Cells[row + 1, 1].Value = "Total Customer:";
                worksheet.Cells[row + 1, 2].Value = CustomerCount;

                // Apply styling for the total count
                worksheet.Cells[row + 1, 1, row + 1, 2].Style.Font.Bold = true;
                // Auto fit columns
                worksheet.Cells.AutoFitColumns();
                package.Save();
                return package.GetAsByteArray();
            }
        }
    }
}
