using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using Microsoft.AspNetCore.Mvc;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.InventoryLiaisonRepServices;
using Ont3010_Project_YA2024.Data.Notifications;

namespace Ont3010_Project_YA2024.Controllers.InventoryLiaison
{
    public class InventoryLiaisonReportController : BaseController
    {
        private readonly GenerateFridgeReportPdf _pdfReportService;

        private readonly GenerateFridgeReportExcel _excelReportService;
        private readonly ApplicationDbContext _context;

        public InventoryLiaisonReportController(BusinessService businessService, ApplicationDbContext context, GenerateFridgeReportPdf pdfReportService,
            GenerateFridgeReportExcel excelReportService,INotificationService notificationService)
             : base(businessService, context, notificationService)
        {
            _pdfReportService = pdfReportService;

            _excelReportService = excelReportService;
            _context = context;
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
            var reportData = await _businessService.GetFridgeReportDataAsync(startDate, endDate);

            // Get all fridges
            var fridges = _context.Fridges.AsQueryable();

            // Apply filtering based on startDate and endDate
            if (parsedStartDate.HasValue)
            {
                fridges = fridges.Where(f => f.CreatedDate >= parsedStartDate.Value);
            }

            if (parsedEndDate.HasValue)
            {
                fridges = fridges.Where(f => f.CreatedDate <= parsedEndDate.Value);
            }

            // Filter for fridges in stock
            fridges = fridges.Where(f => f.IsInStock);

            // Group by CreatedDate to get count per day
            var chartData = fridges
                .GroupBy(f => f.CreatedDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            // Prepare data for the chart
            ViewBag.ChartLabels = chartData.Select(d => d.Date.ToString("yyyy-MM-dd")).ToArray();
            ViewBag.ChartValues = chartData.Select(d => d.Count).ToArray();
            // Group data for pie chart (example: grouping by FridgeType)
            var pieChartData = fridges
                .GroupBy(f => f.ModelType) // Assuming you have a FridgeType property
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToList();

            // Prepare data for the pie chart
            ViewBag.PieChartLabels = pieChartData.Select(d => d.Type).ToArray();
            ViewBag.PieChartValues = pieChartData.Select(d => d.Count).ToArray();
            await SetLayoutData(); // Assuming this sets layout-related data
            await EmployeeNotification();
            return View("Index");
        }




        public async Task<IActionResult> DownloadFridgeReportPdf(string startDate, string endDate)
        {
            // Validate dates
            if (!IsValidDateRange(startDate, endDate))
            {
                return BadRequest("Invalid date range.Please filter before adding");
            }

            // Fetch business details
            var business = await _businessService.GetBusinessAsync();
            var setting = await _businessService.GetSettingAsync();

            if (setting == null)
            {
                return NotFound("Business settings not found.");
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

        public async Task<IActionResult> DownloadFridgeReportWithChartPdf(string startDate, string endDate, string chartBase64)
        {
            if (!IsValidDateRange(startDate, endDate))
            {
                return BadRequest("Invalid date range.");
            }

            // Validate if chartBase64 is provided
            if (string.IsNullOrEmpty(chartBase64))
            {
                return BadRequest("Chart data is missing.");
            }

            // Fetch business details and report data
            var business = await _businessService.GetBusinessAsync();
            var setting = await _businessService.GetSettingAsync();
            var reportData = await _businessService.GetFridgeReportDataAsync(startDate, endDate);

            // Extract chart image bytes from Base64
            byte[] chartImageBytes = Convert.FromBase64String(chartBase64.Split(',')[1]); // Removing "data:image/png;base64,"

            // Generate PDF content, including chart image
            var fileContent = await Task.Run(() =>
                _pdfReportService.CreateFridgeReportPdfWithChart(
                    startDate,
                    endDate,
                    businessName: setting.BusinessName ?? "Default Name",
                    contactEmail: setting.ContactEmail ?? "default@example.com",
                    chartImage: chartImageBytes, // Pass chart image bytes to the PDF service
                    reportData: reportData.ToList()
                )
            );

            return File(fileContent, "application/pdf", "FridgeReportWithChart.pdf");
        }




        private bool IsValidDateRange(string startDate, string endDate)
        {
            // Implement validation logic for date format and range
            return DateTime.TryParse(startDate, out _) && DateTime.TryParse(endDate, out _) && DateTime.Parse(startDate) <= DateTime.Parse(endDate);
        }
    }
}