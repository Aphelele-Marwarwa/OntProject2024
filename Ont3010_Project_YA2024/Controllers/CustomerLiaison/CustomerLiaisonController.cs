using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Controllers.Admin;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;

namespace Ont3010_Project_YA2024.Controllers.CustomerLiaison
{
    [Authorize(Roles = "Customer Liaison")]
    public class CustomerLiaisonController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CustomerLiaisonController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public CustomerLiaisonController(BusinessService businessService, ApplicationDbContext context,
            ILogger<CustomerLiaisonController> logger, UserManager<IdentityUser> userManager, INotificationService notificationService)
             : base(businessService, context, notificationService)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            await SetLayoutData();
            await EmployeeNotification();
           

            // Get the UserID from the current user
            var userId = _userManager.GetUserId(User);

            // Fetch user and employee details
            var user = await _userManager.FindByIdAsync(userId);
            var employee = user != null ? await _context.Employees.FirstOrDefaultAsync(e => e.Email == user.Email) : null;

            if (employee == null)
            {
                // Log and handle error if employee is not found
                _logger.LogError("Employee not found for user {UserId}", userId);
                return NotFound();
            }
            ViewData["FirstName"] = employee.FirstName;
            ViewData["LastName"] = employee.LastName;

            // Fetch data for the dashboard
            var totalCustomers = await _context.Customers.CountAsync();
            var pendingAllocations = await _context.FridgeAllocations.CountAsync(fa => !fa.IsProcessed);
            var totalProcessed = await _context.ProcessAllocations.CountAsync(); // Count of processed requests
            var totalFridges = await GetTotalFridges();

            // Set to zero if counts are less than 0
            ViewData["TotalCustomers"] = totalCustomers > 0 ? totalCustomers : 0;
            ViewData["PendingAllocations"] = pendingAllocations > 0 ? pendingAllocations : 0;
            ViewData["TotalProcessedAllocations"] = totalProcessed > 0 ? totalProcessed : 0; // Explicitly set to 0 if no processed entries
            ViewData["TotalFridges"] = totalFridges > 0 ? totalFridges : 0; // Count of fridges

            // Additional Data
            var fridgeAllocations = await _context.FridgeAllocations
                .Include(fa => fa.Customer)
                .Include(fa => fa.Fridge)
                .OrderByDescending(fa => fa.AllocationDate)
                .ToListAsync();

            var fridgesInStock = await _context.Fridges
                .Where(f => f.IsInStock && !f.IsScrapped && !f.IsDeleted)
                .OrderBy(f => f.ModelType)
                .ToListAsync();

            var recentActivities = await _context.ProcessAllocations
                .OrderByDescending(pa => pa.LastModifiedDate)
                .Take(5)
                .ToListAsync();

            // Pass data to ViewData
            ViewData["FridgeAllocations"] = fridgeAllocations;
            ViewData["FridgesInStock"] = fridgesInStock;
            ViewData["RecentActivities"] = recentActivities;

            // Additional data for chart
            var totalAllocated = fridgeAllocations.Count(fa => fa.IsProcessed);
            var totalPending = pendingAllocations;
            var totalInStock = fridgesInStock.Count;
            ViewData["LastUpdated"] = DateTime.Now;
            ViewData["TotalAllocated"] = totalAllocated;
            ViewData["TotalPending"] = totalPending;
            ViewData["TotalInStock"] = totalInStock;

            _logger.LogInformation("FridgeAllocations: {FridgeAllocationsCount}", fridgeAllocations.Count);

            return View();
        }




        private async Task<int> GetTotalFridges()
        {
            return await _context.Fridges
                .Where(f => !f.IsScrapped && !f.IsDeleted)
                .CountAsync();
        }
    }
}