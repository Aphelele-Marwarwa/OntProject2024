using Microsoft.AspNetCore.Mvc;
using Ont3010_Project_YA2024.Data.CustomerLiaisonRepServices;
using Ont3010_Project_YA2024.Data;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Data.InventoryLiaisonRepServices;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;

namespace Ont3010_Project_YA2024.Controllers.Admin
{
    public class AdminReportingController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly PdfService _pdfService;
        private readonly ExcelService _excelService;
        private readonly GenerateFridgeReportPdf _pdfReportService;
        private readonly GenerateFridgeReportExcel _excelReportService;

        public AdminReportingController(BusinessService businessService, ApplicationDbContext context, PdfService pdfService, ExcelService excelService, GenerateFridgeReportPdf pdfReportService, GenerateFridgeReportExcel excelReportService, INotificationService notificationService)
     : base(businessService, context, notificationService) // Pass notificationService to BaseController
        {
            _context = context;
            _pdfService = pdfService;
            _excelService = excelService;
        }


        public async Task<IActionResult> Index(string startDate, string endDate)
        {
            ViewData["StartDate"] = Request.Query["startDate"].ToString();
            ViewData["EndDate"] = Request.Query["endDate"].ToString();
            DateTime? parsedStartDate = string.IsNullOrEmpty(startDate) ? (DateTime?)null : DateTime.Parse(startDate);
            DateTime? parsedEndDate = string.IsNullOrEmpty(endDate) ? (DateTime?)null : DateTime.Parse(endDate);

            // Fetch report data based on date range
            var reportData = await _businessService.GetFridgeReportDataAsync(startDate, endDate);

            // Get all customers
            var customerQueryable = _context.Customers.AsQueryable();

            // Apply filtering based on startDate and endDate
            if (parsedStartDate.HasValue)
            {
                customerQueryable = customerQueryable.Where(c => c.CreatedDate >= parsedStartDate.Value);
            }

            if (parsedEndDate.HasValue)
            {
                customerQueryable = customerQueryable.Where(c => c.CreatedDate <= parsedEndDate.Value);
            }

            // Group by CreatedDate to get count per day
            var customerChartData = customerQueryable
                .GroupBy(c => c.CreatedDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            // Prepare data for the customer chart
            ViewBag.CustomerChartLabels = customerChartData.Select(d => d.Date.ToString("yyyy-MM-dd")).ToArray();
            ViewBag.CustomerChartValues = customerChartData.Select(d => d.Count).ToArray();

            // Group data for pie chart (example: grouping by FirstName)
            var customerPieChartData = customerQueryable
                .GroupBy(c => c.FirstName) // Assuming you have a property for grouping
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToList();

            // Prepare data for the customer pie chart
            ViewBag.CustomerPieChartLabels = customerPieChartData.Select(d => d.Type).ToArray();
            ViewBag.CustomerPieChartValues = customerPieChartData.Select(d => d.Count).ToArray();

            //-----------------------------------------------------------------------------------------------------------
            // Get all fridges
            var fridgeQueryable = _context.Fridges.AsQueryable();

            // Apply filtering based on startDate and endDate
            if (parsedStartDate.HasValue)
            {
                fridgeQueryable = fridgeQueryable.Where(f => f.CreatedDate >= parsedStartDate.Value);
            }

            if (parsedEndDate.HasValue)
            {
                fridgeQueryable = fridgeQueryable.Where(f => f.CreatedDate <= parsedEndDate.Value);
            }

            // Filter for fridges in stock
            fridgeQueryable = fridgeQueryable.Where(f => f.IsInStock);

            // Group by CreatedDate to get count per day
            var fridgeChartData = fridgeQueryable
                .GroupBy(f => f.CreatedDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            // Prepare data for the fridge chart
            ViewBag.FridgeChartLabels = fridgeChartData.Select(d => d.Date.ToString("yyyy-MM-dd")).ToArray();
            ViewBag.FridgeChartValues = fridgeChartData.Select(d => d.Count).ToArray();

            // Group data for pie chart (example: grouping by ModelType)
            var fridgePieChartData = fridgeQueryable
                .GroupBy(f => f.ModelType) // Assuming you have a property for grouping
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToList();

            // Prepare data for the fridge pie chart
            ViewBag.FridgePieChartLabels = fridgePieChartData.Select(d => d.Type).ToArray();
            ViewBag.FridgePieChartValues = fridgePieChartData.Select(d => d.Count).ToArray();

            await SetLayoutData();
            await EmployeeNotification();
            return View("Index");
        }





        public async Task<IActionResult> DownloadCustomerReportPDF(string startDate, string endDate)
        {
            // Validate dates
            if (!IsValidDateRange(startDate, endDate))
            {
                TempData["ErrorMessage"] = "Invalid date range. Please filter before adding.";
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
            try
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
            catch (Exception ex)
            {
                // Log the error (use a logging framework)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        public async Task<IActionResult> DownloadFridgeReportPdf(string startDate, string endDate)
        {
            // Validate dates
            if (!IsValidDateRange(startDate, endDate))
            {
                TempData["ErrorMessage"] = "Invalid date range. Please filter before adding.";
            }

            // Fetch business details
            var business = await _businessService.GetBusinessAsync();
            var setting = await _businessService.GetSettingAsync();

            if (setting == null)
            {
                TempData["ErrorMessage"] = "Business settings not found.";
            }

            // Fetch report data based on the date range
            var reportData = await _businessService.GetFridgeReportDataAsync(startDate, endDate); // Ensure you implement this method

            string businessName = setting.BusinessName ?? "Default Name";
            string contactEmail = setting.ContactEmail ?? "default@example.com";
            string contactPhone = setting.ContactPhone ?? "000-000-0000";
            string street = setting.Business.Street;
            string city = setting.Business.City;
            string postalCode = setting.Business.PostalCode;
            string stateProvince = setting.Business.StateProvince;
            string country = setting.Business.Country;
            string address = $"{setting.Business.Street}, {setting.Business.City}, {setting.Business.PostalCode}, {setting.Business.StateProvince}, {setting.Business.Country}";
            string formattedAddress = $"{street}\n{city}\n{stateProvince}\n{country}";
            string businessLogoBase64 = setting.BusinessLogo != null ? Convert.ToBase64String(setting.BusinessLogo) : null;

            var fileContent = await Task.Run(() =>
                _pdfReportService.CreateFridgeReportPdf(startDate, endDate, businessName, contactEmail, contactPhone, street, city, postalCode, stateProvince, country, address, businessLogoBase64, reportData));

            return File(fileContent, "application/pdf", "FridgeReport.pdf");
        }


        public async Task<IActionResult> DownloadFridgeReportExcel(string startDate, string endDate)
        {
            // Validate dates
            if (!IsValidDateRange(startDate, endDate))
            {
                return BadRequest("Invalid date range.Please filter before adding");
            }

            try
            {
                // Fetch report data based on the date range
                var reportData = await _businessService.GetFridgeReportDataAsync(startDate, endDate); // Ensure this method returns the required data

                // Generate the Excel report with the required data
                var fileContent = await Task.Run(() => _excelReportService.CreateFridgeReportExcel(startDate, endDate, reportData));

                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FridgeReport.xlsx");
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error while generating report.");
            }
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
