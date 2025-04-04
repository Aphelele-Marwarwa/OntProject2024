using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ont3010_Project_YA2024.Data.InventoryLiaisonRepServices;
using Ont3010_Project_YA2024.Models.admin;
using Ont3010_Project_YA2024.Models.CustomerLiaison;
using System.Threading.Tasks;
using Ont3010_Project_YA2024.Models;
using Ont3010_Project_YA2024.Data.Notifications;

namespace Ont3010_Project_YA2024.Data.Helpers
{
    public class BusinessService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BusinessService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BusinessService(ApplicationDbContext context, ILogger<BusinessService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor; // Assign it to a field
        }

        public async Task<Business?> GetBusinessAsync()
        {
            try
            {
                return await _context.Businesses.FirstOrDefaultAsync(b => b.BusinessId == 1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving business.");
                throw;
            }
        }

        public async Task<Setting?> GetSettingAsync()
        {
            try
            {
                return await _context.Settings.Include(s => s.Business).FirstOrDefaultAsync(s => s.SettingId == 1);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the setting.");
                throw;
            }
        }
        public async Task<List<Customer>> GetCustomerReportDataAsync(string startDate, string endDate)
        {
            // Parse the dates
            DateTime? parsedStartDate = null;
            DateTime? parsedEndDate = null;

            if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out DateTime tempStartDate))
            {
                parsedStartDate = tempStartDate;
            }

            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out DateTime tempEndDate))
            {
                parsedEndDate = tempEndDate;
            }

            // Query the customers based on the date range
            var query = _context.Customers.AsQueryable();

            if (parsedStartDate.HasValue)
            {
                query = query.Where(c => c.CreatedDate >= parsedStartDate.Value);
            }

            if (parsedEndDate.HasValue)
            {
                query = query.Where(c => c.CreatedDate <= parsedEndDate.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<FridgeReportData>> GetFridgeReportDataAsync(string startDate, string endDate)
        {
            // Validate input dates
            if (!DateTime.TryParse(startDate, out var start) || !DateTime.TryParse(endDate, out var end))
            {
                _logger.LogWarning("Invalid date format: StartDate = {startDate}, EndDate = {endDate}", startDate, endDate);
                return Enumerable.Empty<FridgeReportData>(); // Return an empty collection if dates are invalid
            }

            try
            {
                return await _context.Fridges
                    .Where(f => f.CreatedDate >= start && f.CreatedDate <= end)
                    .Select(f => new FridgeReportData
                    {
                        FridgeId = f.FridgeId,
                        SerialNumber = f.SerialNumber,
                        ModelType = f.ModelType,
                        Condition = f.Condition,
                        DoorType = f.DoorType, // New property
                        Size = f.Size, // New property
                        Capacity = f.Capacity, // New property
                        SupplierName = f.SupplierName, // New property
                        SupplierContact = f.SupplierContact, // New property
                        IsInStock = f.IsInStock, // New property
                        WarrantyEndDate = f.WarrantyEndDate,
                        CreatedDate = f.CreatedDate// New property
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error retrieving fridge report data for dates {startDate} to {endDate}.", startDate, endDate);
                throw; // Rethrow or handle as needed
            }
        }

        private string GetCurrentUserEmail()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name; // Get the user's email
        }

        private async Task CreateEmployeeNotificationAsync(Employee newEmployee)
        {
            // Step 1: Get the EmployeeId of the newly created employee
            var brendEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == newEmployee.Email);

            // Step 2: Get the ActionBy (the person who created the employee)
            var userEmail = GetCurrentUserEmail(); // Get the current user's email
            var actionByEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == userEmail);

            var actionBy = actionByEmployee != null ?
                $"{actionByEmployee.FirstName} {actionByEmployee.LastName}" :
                userEmail; // Fallback to email if name is not found

            // Step 3: Create a notification after employee is created
            var notification = new Notification
            {
                Message = $"Employee {brendEmployee.FirstName} {brendEmployee.LastName} was created successfully.", // Use the correct employee's name
                ActionBy = actionBy,  // Use the full name of the user who created the employee
                EmployeeId = brendEmployee.EmployeeId, // Use the correct employee Id from the database
                Date = DateTime.Now
            };

            // Step 4: Add notification to database
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync(); // Save notification first to get the ID

            // Step 5: Fetch all administrators
            var administrators = await _context.Employees
                .Where(e => e.Role == "Administrator")
                .ToListAsync();

            // Step 6: Create EmployeeNotificationStatus for all administrators
            foreach (var admin in administrators)
            {
                var status = new EmployeeNotificationStatus
                {
                    EmployeeId = admin.EmployeeId,
                    NotificationId = notification.Id,
                    IsRead = false // Set IsRead to false by default for all admins
                };

                // Mark as read if the admin is the one who created the employee
                if (admin.Email == userEmail)
                {
                    status.IsRead = true;
                }

                _context.EmployeeNotificationStatuses.Add(status);
            }

            // Step 7: Save changes for notification statuses
            await _context.SaveChangesAsync();
        }


    }
}
