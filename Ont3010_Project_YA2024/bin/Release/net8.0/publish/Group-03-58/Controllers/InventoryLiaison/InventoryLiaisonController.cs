using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Controllers.CustomerLiaison;
using Ont3010_Project_YA2024.Models.InventoryLiaison;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Models;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;

namespace Ont3010_Project_YA2024.Controllers.InventoryLiaison
{
    [Authorize(Roles = "Inventory Liaison")]
    public class InventoryLiaisonController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InventoryLiaisonController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public InventoryLiaisonController(BusinessService businessService ,ApplicationDbContext context, ILogger<InventoryLiaisonController> logger, UserManager<IdentityUser> userManager, INotificationService notificationService)
             : base(businessService, context, notificationService)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
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

            // Retrieve dashboard data
            var totalFridges = await _context.Fridges.CountAsync(f => !f.IsDeleted);
            var fridgesInStock = await _context.Fridges.CountAsync(f => f.IsInStock && !_context.ScrappedFridges.Any(sf => sf.FridgeId == f.FridgeId));
            var fridgesScrapped = await _context.ScrappedFridges.CountAsync();
            var totalPurchaseRequests = await _context.PurchaseRequests.CountAsync();
            var approvedRequests = await _context.PurchaseRequests.CountAsync(r => r.Status == RequestStatus.Approved);
            var pendingRequests = await _context.PurchaseRequests.CountAsync(r => r.Status == RequestStatus.Pending);
            var processedAllocations = await _context.FridgeAllocations.CountAsync(a => a.IsProcessed);
            var pendingAllocations = await _context.FridgeAllocations.CountAsync(a => !a.IsProcessed);

            var dashboardViewModel = new DashboardViewModel
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                TotalFridges = totalFridges,
                FridgesInStock = fridgesInStock,
                FridgesScrapped = fridgesScrapped,
                TotalPurchaseRequests = totalPurchaseRequests,
                ApprovedRequests = approvedRequests,
                PendingRequests = pendingRequests,
                ProcessedAllocations = processedAllocations,
                PendingAllocations = pendingAllocations,
                LastUpdated = DateTime.Now,
                PurchaseRequests = await _context.PurchaseRequests.ToListAsync(),
                FridgeAllocations = await _context.FridgeAllocations.ToListAsync()

            };

          

            return View(dashboardViewModel);
        }


    }
}