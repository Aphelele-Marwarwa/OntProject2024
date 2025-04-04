using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.CustomerLiaisonRepServices;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Models.CustomerLiaison;

namespace Ont3010_Project_YA2024.Controllers.CustomerLiaison
{
    public class CustomerLiaisonReportingController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly PdfService _pdfService;
        private readonly ExcelService _excelService;

        public CustomerLiaisonReportingController(BusinessService businessService, ApplicationDbContext context, PdfService pdfService, ExcelService excelService, INotificationService notificationService)
            : base(businessService, context, notificationService)
        {
            _context = context;
            _pdfService = pdfService;
            _excelService = excelService;
        }

        public async Task<IActionResult> Index(string startDate, string endDate)
        {
            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;

            // Parse start and end dates with error handling
            DateTime? parsedStartDate = null;
            DateTime? parsedEndDate = null;

            if (!string.IsNullOrEmpty(startDate))
            {
                if (DateTime.TryParse(startDate, out DateTime tempStartDate))
                {
                    parsedStartDate = tempStartDate;
                }
                else
                {
                    // Handle invalid startDate format
                    ModelState.AddModelError("StartDate", "Invalid start date format");
                }
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                if (DateTime.TryParse(endDate, out DateTime tempEndDate))
                {
                    parsedEndDate = tempEndDate;
                }
                else
                {
                    // Handle invalid endDate format
                    ModelState.AddModelError("EndDate", "Invalid end date format");
                }
            }

            // Fetch report data
            var reportData = await _businessService.GetCustomerReportDataAsync(startDate, endDate);

            // Get all customers
            var customers = _context.Customers.AsQueryable();

            // Apply filtering based on startDate and endDate
            if (parsedStartDate.HasValue)
            {
                customers = customers.Where(c => c.CreatedDate >= parsedStartDate.Value);
            }

            if (parsedEndDate.HasValue)
            {
                customers = customers.Where(c => c.CreatedDate <= parsedEndDate.Value);
            }

            // Group by CreatedDate to get count per day
            var chartData = customers
                .GroupBy(c => c.CreatedDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            // Prepare data for the chart
            ViewBag.ChartLabels = chartData.Select(d => d.Date.ToString("yyyy-MM-dd")).ToArray();
            ViewBag.ChartValues = chartData.Select(d => d.Count).ToArray();

            // Group data for pie chart (example: grouping by CustomerType)
            var pieChartData = customers
                .GroupBy(c => c.FirstName) // Assuming you have a CustomerType property
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToList();

            // Prepare data for the pie chart
            ViewBag.PieChartLabels = pieChartData.Select(d => d.Type).ToArray();
            ViewBag.PieChartValues = pieChartData.Select(d => d.Count).ToArray();

            await SetLayoutData(); // Assuming this sets layout-related data
            await EmployeeNotification();
          
            return View("Index");
        }


        // Detailed Customer Report
        public async Task<IActionResult> DownloadCustomerReportPDF(string startDate, string endDate)
        {
            // Validate dates
            if (!IsValidDateRange(startDate, endDate))
            {
                TempData["ErrorMessage"] = "Invalid date range. Please enter a valid start and end date.";
            }

            // Fetch business details
            var setting = await _businessService.GetSettingAsync();
            if (setting == null)
            {
                TempData["ErrorMessage"] = "Business settings not found.";
            }

            // Build the customer query
            var query = _context.Customers.AsQueryable();

            if (DateTime.TryParse(startDate, out var start))
            {
                query = query.Where(c => c.CreatedDate >= start);
            }

            if (DateTime.TryParse(endDate, out var end))
            {
                query = query.Where(c => c.CreatedDate <= end);
            }

            // Fetch customers who are not deleted
            var customers = await query.Where(c => !c.IsDeleted).ToListAsync();

            // Prepare address components
            string businessName = setting.BusinessName ?? "Default Name";
            string contactEmail = setting.ContactEmail ?? "default@example.com";
            string contactPhone = setting.ContactPhone ?? "000-000-0000";
            string street = setting.Business.Street;
            string city = setting.Business.City;
            string postalCode = setting.Business.PostalCode;
            string stateProvince = setting.Business.StateProvince;
            string country = setting.Business.Country;
            string address = $"{street}, {city}, {postalCode}, {stateProvince}, {country}";

            // Prepare the business logo
            string businessLogoBase64 = GetBusinessLogoBase64();

            // Generate PDF
            var pdfBytes = await _pdfService.GenerateCustomerReportPdfAsync(customers, startDate, endDate, businessName, contactEmail, contactPhone, street, city, postalCode, stateProvince, country, address, businessLogoBase64);

            return File(pdfBytes, "application/pdf", "CustomerReport.pdf");
        }



        public async Task<IActionResult> DownloadCustomerReportEXCL(string startDate, string endDate)
        {
            var query = _context.Customers.AsQueryable();

            if (DateTime.TryParse(startDate, out var start))
            {
                query = query.Where(c => c.CreatedDate >= start);
            }

            if (DateTime.TryParse(endDate, out var end))
            {
                query = query.Where(c => c.CreatedDate <= end);
            }

            var customers = await query.Where(c => !c.IsDeleted).ToListAsync();
            var excelBytes = await _excelService.GenerateCustomerReportExcelAsync(customers);

            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CustomerReport.xlsx");

        }

        private string GetBusinessLogoBase64()
        {
            // Retrieve Base64 string from settings or database
            var setting = _context.Settings.FirstOrDefault();
            if (setting != null && setting.BusinessLogo != null)
            {
                return Convert.ToBase64String(setting.BusinessLogo);
            }
            return string.Empty; // Return an empty string or handle the case where the logo is not found
        }

        private bool IsValidDateRange(string startDate, string endDate)
        {
            // Implement validation logic for date format and range
            return DateTime.TryParse(startDate, out _) && DateTime.TryParse(endDate, out _) && DateTime.Parse(startDate) <= DateTime.Parse(endDate);
        }
    }
}
