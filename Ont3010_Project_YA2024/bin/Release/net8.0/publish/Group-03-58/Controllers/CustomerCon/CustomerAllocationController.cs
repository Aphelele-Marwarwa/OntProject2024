using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ont3010_Project_YA2024.Data;
using Ont3010_Project_YA2024.Data.Helpers;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Models.CustomerLiaison;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ont3010_Project_YA2024.Controllers.CustomerCon
{
    [Authorize(Roles = "Customer")]
    public class CustomerAllocationController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CustomerAllocationController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public CustomerAllocationController(
            BusinessService businessService,
            ApplicationDbContext context,
            ILogger<CustomerAllocationController> logger,
            UserManager<IdentityUser> userManager, INotificationService notificationService)
            : base(businessService, context, notificationService)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string searchString, string sortOrder, string sortDirection, int page = 1)
        {
            // Get the currently logged-in user's email
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            // Retrieve the customer associated with the logged-in user's email
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.EmailAddress == userEmail);

            // Check if the customer exists
            if (customer == null)
            {
                ViewBag.Message = "Customer not found.";
                return View(); // Redirect to a view that informs the user
            }

            // Retrieve fridge allocations for this customer
            var fridgeAllocationsQuery = _context.FridgeAllocations
                .Include(f => f.Fridge)
                .Where(f => f.CustomerId == customer.CustomerId);

            // Filter by search string if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                fridgeAllocationsQuery = fridgeAllocationsQuery.Where(f => f.Fridge.ModelType.Contains(searchString));
            }

            // Sort by the selected order
            sortOrder = string.IsNullOrEmpty(sortOrder) ? "ModelType" : sortOrder;
            sortDirection = string.IsNullOrEmpty(sortDirection) ? "asc" : sortDirection;

            switch (sortOrder)
            {
                case "AllocationDate":
                    fridgeAllocationsQuery = sortDirection == "asc"
                        ? fridgeAllocationsQuery.OrderBy(f => f.AllocationDate)
                        : fridgeAllocationsQuery.OrderByDescending(f => f.AllocationDate);
                    break;
                default:
                    fridgeAllocationsQuery = sortDirection == "asc"
                        ? fridgeAllocationsQuery.OrderBy(f => f.Fridge.ModelType)
                        : fridgeAllocationsQuery.OrderByDescending(f => f.Fridge.ModelType);
                    break;
            }

            // Pagination setup
            int pageSize = 10;
            var fridgeAllocations = await fridgeAllocationsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Total count for pagination
            int totalAllocations = await fridgeAllocationsQuery.CountAsync();

            // Pass pagination and sort information to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalAllocations / (double)pageSize);
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentDirection = sortDirection;

            // Check if there are any allocations
            if (!fridgeAllocations.Any())
            {
             
                await SetLayoutData();
                return View(fridgeAllocations);
            }

            await SetLayoutData();

            // Pass the list of fridge allocations to the Index view
            return View(fridgeAllocations);
        }


        [HttpGet]
        public async Task<IActionResult> ViewAllocationDetail(int id)
        {
            var allocation = await _context.FridgeAllocations
                .Include(f => f.Fridge)
                .Include(f => f.ProcessAllocations)
                .FirstOrDefaultAsync(a => a.FridgeAllocationId == id);

            if (allocation == null)
            {
                return NotFound();
            }

            await SetLayoutData();
            return View(allocation);
        }
    }
}
