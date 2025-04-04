using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Models.InventoryLiaison;
using Ont3010_Project_YA2024.Models;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;

[Authorize(Roles = "Administrator")]
public class AdminController : BaseController
{
    private readonly ILogger<AdminController> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(BusinessService businessService, ApplicationDbContext context, ILogger<AdminController> logger, UserManager<IdentityUser> userManager, INotificationService notificationService)
        : base(businessService, context, notificationService) // Ensure these parameters match the BaseController constructor
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        await SetLayoutData(); // Use the shared method from BaseController
       await EmployeeNotification();
        // Get the UserID from the current user
        var userId = _userManager.GetUserId(User);

        // Fetch the user from the UserManager
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            // Handle the case where the user is not found
            return NotFound();
        }

        // Retrieve FirstName and LastName properties from Employee model
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == user.Email);

        if (employee == null)
        {
            // Handle the case where the employee is not found
            return NotFound();
        }

        // Retrieve dashboard data for Admin
        var totalCustomers = await _context.Customers.CountAsync();
        var totalFridges = await _context.Fridges.CountAsync(f => !f.IsDeleted);
        var fridgesInStock = await _context.Fridges.CountAsync(f => f.IsInStock && !_context.ScrappedFridges.Any(sf => sf.FridgeId == f.FridgeId));
        var fridgesScrapped = await _context.ScrappedFridges.CountAsync();
        var totalPurchaseRequests = await _context.PurchaseRequests.CountAsync();
        var approvedRequests = await _context.PurchaseRequests.CountAsync(r => r.Status == RequestStatus.Approved);
        var pendingRequests = await _context.PurchaseRequests.CountAsync(r => r.Status == RequestStatus.Pending);
        var totalProcessed = await _context.ProcessAllocations.CountAsync(); // Count of processed requests
        var pendingAllocations = await _context.FridgeAllocations.CountAsync(a => !a.IsProcessed);
        var totalEmployees = await _context.Employees.CountAsync(); // New line for total employees

        var dashboardViewModel = new DashboardViewModel
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            TotalCustomers = totalCustomers,
            TotalFridges = totalFridges,
            FridgesInStock = fridgesInStock,
            FridgesScrapped = fridgesScrapped,
            TotalPurchaseRequests = totalPurchaseRequests,
            ApprovedRequests = approvedRequests,
            PendingRequests = pendingRequests,
            Metrics = new List<int>
            {
                totalCustomers,
                totalFridges,
                fridgesInStock,
                pendingAllocations
            },
           
            PendingAllocations = pendingAllocations,
            TotalEmployees = totalEmployees, // Set the total employees property
            LastUpdated = DateTime.Now,
            PurchaseRequests = await _context.PurchaseRequests.ToListAsync(),
            FridgeAllocations = await _context.FridgeAllocations.ToListAsync(),
            FridgesInStockList = await _context.Fridges
                .Where(f => f.IsInStock && !f.IsScrapped && !f.IsDeleted)
                .OrderBy(f => f.ModelType)
                .ToListAsync(),
            RecentActivities = await _context.FridgeAllocations // Placeholder, update as necessary
                .OrderByDescending(fa => fa.AllocationDate)
                .Take(5)
                .ToListAsync()
        };

        ViewData["TotalProcessedAllocations"] = totalProcessed > 0 ? totalProcessed : 0;


        // Add notifications to ViewBag for displaying in the view

        ViewBag.ShowPendingAllocationsAlert = pendingAllocations > 0;

        _logger.LogInformation("Dashboard data retrieved for admin");

        return View(dashboardViewModel);
    }


}
