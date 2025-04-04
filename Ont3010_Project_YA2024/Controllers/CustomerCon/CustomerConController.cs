using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Models;
using Ont3010_Project_YA2024.Models.CustomerLiaison;
using System.Security.Claims;

namespace Ont3010_Project_YA2024.Controllers.CustomerCon
{
    [Authorize(Roles = "Customer")]
    public class CustomerConController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CustomerConController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public CustomerConController(
            BusinessService businessService,
            ApplicationDbContext context,
            ILogger<CustomerConController> logger,
            UserManager<IdentityUser> userManager, INotificationService notificationService)
            : base(businessService, context, notificationService)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get the logged-in customer's email
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.EmailAddress == userEmail);

            if (customer == null)
            {
                ViewBag.Message = "Customer not found.";
                return View();
            }

            // Fetch all fridge allocations for this customer
            var fridgeAllocations = await _context.FridgeAllocations
                .Include(f => f.Fridge)
                .Where(f => f.CustomerId == customer.CustomerId)
                .ToListAsync();

            // Fetch all processed allocations for comparison
            var processedAllocationIds = await _context.ProcessAllocations
                .Where(pa => pa.FridgeAllocationId > 0) // Ensure it's a valid ID
                .Select(pa => pa.FridgeAllocationId)
                .ToListAsync();

            // Count processed and unprocessed allocations
            var processedAllocations = fridgeAllocations
                .Count(f => processedAllocationIds.Contains(f.FridgeAllocationId));

            var unprocessedAllocations = fridgeAllocations.Count - processedAllocations;

            // Prepare the data for the graph
            var allocationData = new AllocationGraphData
            {
                Processed = processedAllocations,
                Unprocessed = unprocessedAllocations
            };

            // Set up layout and view data
            await SetLayoutData();
            await CustomerNotification();
            // Pass the customer, allocations data, and graph data to the view
            var viewModel = new CustomerDashboardViewModel
            {
                Customer = customer,
                RecentFridgeAllocations = fridgeAllocations.Take(5).ToList(), // Take 5 most recent
                TotalAllocations = fridgeAllocations.Count,
                ProcessedAllocations = processedAllocations,
                UnprocessedAllocations = unprocessedAllocations,
                AllocationGraphData = allocationData
            };
          
            var totalNewFridgeRequests = _context.NewFridgeRequests.Count(f => f.CustomerId == customer.CustomerId );

           
            ViewData["TotalNewFridgeRequests"] = totalNewFridgeRequests;

            // Fetch all customer reports and fridge requests if needed
           
            var fridgeRequests = _context.NewFridgeRequests.Where(f => f.CustomerId == customer.CustomerId).ToList();

            // Pass the data to the view (without a ViewModel)
          
            ViewBag.FridgeRequests = fridgeRequests;
            return View(viewModel);
        }





    }
}
